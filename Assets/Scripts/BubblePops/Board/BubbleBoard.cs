using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BubblePops.ScriptableObjects;
using System;
using BubblePops.BubbleShooter;

namespace BubblePops.Board
{
    public class BubbleBoard : MonoBehaviour
	{
		[Header("Board Settings")]
		[SerializeField] int _boardWidth;
		[SerializeField] int _boardHeight;
		[SerializeField] BubbleConfig _bubbleConfig;

		[Header("Prefabs")]
		[SerializeField] BubbleSlotView _bubbleSlotPrefab;
		[SerializeField] BubbleDropView _bubbleDropPrefab;

		[Header("Gameobject References")]
		[SerializeField] Transform _bubbleSlotsContainer;
		[SerializeField] GameObject LeftSideBouncer;
		[SerializeField] GameObject RightSideBouncer;

		[Header("Behaviour Refernces")]
		[SerializeField] BubbleAim _bubbleAim;
		[SerializeField] BubbleAmmo _bubbleAmmo;
		[SerializeField] BubbleShoot _bubbleShoot;
		[SerializeField] Score _score;
		[SerializeField] BubbleCatcher _bubbleCatcher;

		private BubbleSlot[] _bubbleSlots;
		private float _bubbleSize;
		private BubbleSlotView _activatedSlotPreview;

		void Start()
		{
			_bubbleAim.OnBubbleSlotPreviewActivated += ActivateBubbleSlotPreview;
        	_bubbleAim.OnNoBubbleSlotPreviewActivated += DeactivateBubbleSlotPreview;
			_bubbleAim.OnBubbleShot += BubbleShot;

			_bubbleShoot.OnBubbleAdded += BubbleAddedToBoard;

			_bubbleCatcher.OnBubbleDropped += BubbleDropped;
		}

		private void ActivateBubbleSlotPreview(BubbleSlotView bubbleSlot)
		{
			if (_activatedSlotPreview == bubbleSlot)
				return;

			DeactivateBubbleSlotPreview();

			_activatedSlotPreview = bubbleSlot;
			_activatedSlotPreview.ActivatePreview();
		}

		private void DeactivateBubbleSlotPreview()
		{
			if (_activatedSlotPreview != null)
			{
				_activatedSlotPreview.DeactivatePreview();
				_activatedSlotPreview = null;
			}
		}

		private void BubbleShot()
		{
			if (_bubbleSlots.Skip(Math.Max(0, _bubbleSlots.Length - _boardWidth)).Any(slot => slot.HasBubble() || slot.IsReserved()))
			{
				AddNewRow();
			}
		}

		private void BubbleDropped(int number)
		{
			_score.AddScore(number);
		}

		public void Create()
		{
			_bubbleSlots = new BubbleSlot[_boardHeight * _boardWidth];
			_bubbleSize = _bubbleSlotPrefab.GetComponent<Renderer>().bounds.size.x;

			for (int y = 0, i = 0; y < _boardHeight; y++)
			{
				for (int x = 0 ; x < _boardWidth ; x++)
				{
					CreateBubbleSlot(x, y, i++);
				}
			}

			CenterBoard();
			PlaceSideBouncers();
			_bubbleCatcher.transform.position = _bubbleAim.transform.position;

			CreateInitialBoardBubbles();
			_bubbleAmmo.Setup();
		}

		private void CreateBubbleSlot(int x, int y, int i)
		{
			Vector3 position;
			position.x = (x + y * 0.5f - y / 2) * _bubbleSize;
			position.y = -y * _bubbleSize;
			position.z = 0f;

			var bubbleSlotView = Instantiate<BubbleSlotView>(_bubbleSlotPrefab);
			bubbleSlotView.transform.SetParent(_bubbleSlotsContainer, false);
			bubbleSlotView.transform.localPosition = position;

			_bubbleSlots[i] = new BubbleSlot(bubbleSlotView);
		}

		private void CenterBoard()
		{	
			var x = -_bubbleSize * _boardWidth / 2f + _bubbleSize / 4f;
			var y = _bubbleSize * _boardHeight / 1.5f;
			_bubbleSlotsContainer.position = new Vector3(x,y,0f);
		}

		private void PlaceSideBouncers()
		{
			var offset = _bubbleSize * _boardWidth / 2f - _bubbleSize / 4f;
			RightSideBouncer.transform.position += Vector3.right * offset;
			LeftSideBouncer.transform.position += Vector3.left * offset;
		}

		private void CreateInitialBoardBubbles()
		{
			var random = new System.Random();
			var bubbleConfigs = _bubbleConfig.configs.Where(config => config.number < 1024).ToList();
			var allRowsButFirst = _bubbleSlots.Take(_bubbleSlots.Length - _boardWidth);
			foreach (var bubbleSlot in allRowsButFirst) 
			{
				bubbleSlot.PlaceBubble(bubbleConfigs[random.Next(bubbleConfigs.Count)]);
			}
		}

		private void BubbleAddedToBoard(BubbleSlotView bubbleSlotView, BubbleConfigItem bubbleConfig)
		{
			var bubbleSlot = _bubbleSlots[Array.IndexOf(_bubbleSlots,bubbleSlotView.BubbleSlot())];
			bubbleSlot.PlaceBubble(bubbleConfig); 

			var adjacentBubblesWithSameScore = GetAdjacentBubbles(bubbleSlot, new List<BubbleSlot> { bubbleSlot });
			if (adjacentBubblesWithSameScore.Count > 1)
			{
				_score.AddScore(bubbleConfig.number * adjacentBubblesWithSameScore.Count);
				var newBubbleNumber = MergeBubbles(bubbleConfig.number, adjacentBubblesWithSameScore.Count);
				var bubbleToMerge = adjacentBubblesWithSameScore.Last();
				foreach (var adjacent in adjacentBubblesWithSameScore) 
				{
					if (HasAdjacentWithNumber(adjacent, newBubbleNumber)) 
					{
						bubbleToMerge = adjacent;
						break;
					}
				}

				var otherAdjacentsToPop = adjacentBubblesWithSameScore.Where(adjacent => adjacent.Id() != bubbleToMerge.Id()).ToList();
				foreach (var otherAdjacent in otherAdjacentsToPop)
				{
					otherAdjacent.Pop();
				}

				RemoveEmptyRows();

				var newBubbleConfig = _bubbleConfig.configs.FirstOrDefault(config => config.number == newBubbleNumber);
				BubbleAddedToBoard(bubbleToMerge.View(), newBubbleConfig);
			}
			else 
			{
				SearchHangingBubbles();
			}
		}

        private void SearchHangingBubbles()
        {
			var firstEmptySlotIndex = Array.IndexOf(_bubbleSlots, _bubbleSlots.FirstOrDefault(slot => slot.IsEmpty()));
			for (var i = firstEmptySlotIndex; i < _bubbleSlots.Length; i++) 
			{
				var bubbleSlot = _bubbleSlots[i];
				if (bubbleSlot.IsEmpty())
					continue;

				if (!HasCeiling(i, bubbleSlot))
				{
					var dropBubble = Instantiate(_bubbleDropPrefab, bubbleSlot.View().transform.position, Quaternion.identity);
					dropBubble.SetBubbleConfig(bubbleSlot.BubbleConfig());
					bubbleSlot.Pop();
				}
			}	
        }

        private bool HasCeiling(int slotIndex, BubbleSlot bubbleSlot)
        {
			var currentRowIndex = Mathf.Floor(slotIndex / _boardWidth);
			var isEvenRow = currentRowIndex % 2 == 0;
			var previousRowIndex = currentRowIndex - 1;
			if (previousRowIndex >= 0) 
        	{
				var minIndexInPreviousRow = previousRowIndex * _boardWidth;
            	var maxIndexInPreviousRow = minIndexInPreviousRow + (_boardWidth - 1);

				if (_bubbleSlots[slotIndex - _boardWidth].HasBubble())
					return true;

				var otherLowerIndex = slotIndex - (_boardWidth +(isEvenRow ? 1 : -1));
				if (otherLowerIndex >= minIndexInPreviousRow && otherLowerIndex <= maxIndexInPreviousRow) 
				{
					if (_bubbleSlots[otherLowerIndex].HasBubble())
						return true;
				}
			}

			return false;
        }

        private bool HasNoAdjacents(BubbleSlot bubbleSlot)
        {
            var adjacentIndexes = GetAdjacentIndexes(bubbleSlot);
			foreach (var i in adjacentIndexes)
			{
				print(i);
			}
			return adjacentIndexes.Select(i => _bubbleSlots[i]).Where(slot => slot.HasBubble()).Count() == 0;
        }

        private void RemoveEmptyRows()
        {
			var lastIndexWithoutBubble = _bubbleSlots.Length - 1;
            for (var i = _bubbleSlots.Length - 1; i >= 0; i--) 
			{
				if (_bubbleSlots[i].HasBubble() || _bubbleSlots[i].IsReserved())
					break;
				else
					lastIndexWithoutBubble = i;
			}

			var emptyRowsCount = (_bubbleSlots.Length - 1 - lastIndexWithoutBubble) / _boardWidth;
			if (emptyRowsCount == 0)
				return;

			var rowsToDelete = emptyRowsCount - 1;

			foreach (var bubbleSlot in _bubbleSlots.Reverse().Take(rowsToDelete * _boardWidth)) 
			{
				Destroy(bubbleSlot.View());
			}

			var updatedBubbleSlots = new BubbleSlot[_bubbleSlots.Length - rowsToDelete * _boardWidth];
			Array.Copy(_bubbleSlots, updatedBubbleSlots, updatedBubbleSlots.Length);
			_bubbleSlots = updatedBubbleSlots;

			_bubbleSlotsContainer.position -= Vector3.up * _bubbleSize * rowsToDelete;
        }

        private bool HasAdjacentWithNumber(BubbleSlot bubbleSlot, int bubbleNumber)
        {
            var adjacentIndexes = GetAdjacentIndexes(bubbleSlot);
			foreach (var adjacentBubbleIndex in adjacentIndexes) 
			{
				var adjacentBubble = _bubbleSlots[adjacentBubbleIndex];
				if (adjacentBubble.HasBubble() && adjacentBubble.BubbleConfig().number == bubbleNumber)
				{
					return true;
				}
			}

			return false;
        }

        private void AddNewRow()
		{
			var updatedBubbleSlots = new BubbleSlot[_bubbleSlots.Length + _boardWidth];
			Array.Copy(_bubbleSlots, updatedBubbleSlots, _bubbleSlots.Length);
			_bubbleSlots = updatedBubbleSlots;
			
			_bubbleSlotsContainer.position += Vector3.up * _bubbleSize;
			
			var newRowIndex = _bubbleSlots.Length / _boardWidth - 1;
			for (int x = 0, i = updatedBubbleSlots.Length - _boardWidth; x < _boardWidth; x++)
			{
				CreateBubbleSlot(x, newRowIndex, i++);
			}
		}

		private List<BubbleSlot> GetAdjacentBubbles(BubbleSlot bubbleSlot, List<BubbleSlot> knownAdjacents)
		{
			var newAdjacentBubbleSlots = new List<BubbleSlot>();
			var adjacentIndexes = GetAdjacentIndexes(bubbleSlot);
			
			foreach (var adjacentBubbleIndex in adjacentIndexes) 
			{
				var bubbleSlotToAdd = _bubbleSlots[adjacentBubbleIndex];
				if (bubbleSlotToAdd.HasBubble() && bubbleSlotToAdd.BubbleConfig().number == bubbleSlot.BubbleConfig().number)
				{
					if (!knownAdjacents.Any(slot => slot.Id() == bubbleSlotToAdd.Id()))
						newAdjacentBubbleSlots.Add(bubbleSlotToAdd);
				}
			}

			if (newAdjacentBubbleSlots.Count() == 0)
				return knownAdjacents;

			var result = new List<BubbleSlot>();
			foreach (var newSlot in newAdjacentBubbleSlots) 
			{
				var newSlotAdjacentBubbles = GetAdjacentBubbles(newSlot, knownAdjacents.Concat(newAdjacentBubbleSlots).ToList());
				foreach(var newSlotAdjacentBubble in newSlotAdjacentBubbles)
				{
					if (!result.Any(slot => slot.Id() == newSlotAdjacentBubble.Id()))
						result.Add(newSlotAdjacentBubble);
				}	
			}
			return result;
		}

		private List<int> GetAdjacentIndexes(BubbleSlot bubbleSlot)
		{
			var indexes = new List<int>();
			
			var slotIndex = Array.IndexOf(_bubbleSlots, bubbleSlot);

			// adjacents in same row
			var currentRowIndex = Mathf.Floor(slotIndex / _boardWidth);
			var minIndexInRow = currentRowIndex * _boardWidth;
			var maxIndexInRow = minIndexInRow + (_boardWidth-1);

			if (slotIndex - 1 >= minIndexInRow)
				indexes.Add(slotIndex-1);

			if (slotIndex + 1 <= maxIndexInRow)
				indexes.Add(slotIndex+1);

			var isEvenRow = currentRowIndex % 2 == 0;

			// adjacents in previous row
			var previousRowIndex = currentRowIndex - 1;
			if (previousRowIndex >= 0) 
        	{
				var minIndexInPreviousRow = previousRowIndex * _boardWidth;
            	var maxIndexInPreviousRow = minIndexInPreviousRow + (_boardWidth - 1);

				indexes.Add(slotIndex - _boardWidth);

				var otherLowerIndex = slotIndex - (_boardWidth +(isEvenRow ? 1 : -1));
				if (otherLowerIndex >= minIndexInPreviousRow && otherLowerIndex <= maxIndexInPreviousRow) 
				{
					indexes.Add(otherLowerIndex);
				}
			}

			// adjacents in next row
			var nextRowIndex = currentRowIndex + 1;
			var boardHeight = _bubbleSlots.Length / _boardWidth;
			if (nextRowIndex <= boardHeight - 1) 
			{
				var minIndexInNextRow = nextRowIndex * _boardWidth;
				var maxIndexInNextRow = minIndexInNextRow + (_boardWidth-1);

				indexes.Add(slotIndex + _boardWidth);

				var otherUpperIndex = slotIndex + (_boardWidth + (isEvenRow ? -1 : 1));
				if (otherUpperIndex >= minIndexInNextRow && otherUpperIndex <= maxIndexInNextRow)
				{
					indexes.Add(otherUpperIndex);
				}
			}

			return indexes;
		}

		private int MergeBubbles(int number, int times)
		{
			return number * (int)Math.Pow(2, times - 1);
		}
	}


}
