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

		[Header("Gameobject References")]
		[SerializeField] Transform _bubbleSlotsContainer;
		[SerializeField] GameObject LeftSideBouncer;
		[SerializeField] GameObject RightSideBouncer;

		[Header("Behaviour Refernces")]
		[SerializeField] BubbleAim _bubbleAim;
		[SerializeField] BubbleAmmo _bubbleAmmo;
		[SerializeField] BubbleShoot _bubbleShoot;

		private BubbleSlot[] _bubbleSlots;
		private float _bubbleSize;
		private BubbleSlotView _activatedSlotPreview;

		void Start()
		{
			_bubbleAim.OnBubbleSlotPreviewActivated += ActivateBubbleSlotPreview;
        	_bubbleAim.OnNoBubbleSlotPreviewActivated += DeactivateBubbleSlotPreview;
			_bubbleAim.OnBubbleShot += BubbleShot;

			_bubbleShoot.OnBubbleAdded += BubbleAddedToBoard;
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

			CenterGrid();
			PlaceSideBouncers();

			CreateInitialBoardBubbles();
			_bubbleAmmo.Setup();
		}

		private void CreateBubbleSlot(int x, int y, int i)
		{
			Vector3 position;
			position.x = (x + y * 0.5f - y / 2) * _bubbleSize;
			position.y = -y * _bubbleSize;// - _bubbleSlotsContainer.position.y;
			position.z = 0f;

			var bubbleSlotView = Instantiate<BubbleSlotView>(_bubbleSlotPrefab);
			bubbleSlotView.transform.SetParent(_bubbleSlotsContainer, false);
			bubbleSlotView.transform.localPosition = position;

			_bubbleSlots[i] = new BubbleSlot(bubbleSlotView);
		}

		private void CenterGrid()
		{	
			var x = -_bubbleSize * _boardWidth / 2f - _bubbleSize / 4f;
			var y = _bubbleSize * _boardHeight;
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
			var bubbleConfigs = _bubbleConfig.configs;
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

			var adjacentBubbles = GetAdjacentBubbles(bubbleSlot, new List<BubbleSlot> { bubbleSlot });
			foreach (var adjacent in adjacentBubbles) {
				print(adjacent.BubbleConfig().number);           
				// TODO: calculate score
			}
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
			if (nextRowIndex <= boardHeight) 
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
	}


}
