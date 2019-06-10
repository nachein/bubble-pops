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

		public void Create()
		{
			_bubbleSlots = new BubbleSlot[_boardHeight * _boardWidth];
			_bubbleSize = _bubbleSlotPrefab.GetComponent<Renderer>().bounds.size.x;

			for (int y = 0, i = 0; y < _boardHeight; y++)
			{
				for (int x = 0; x < _boardWidth; x++)
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
			position.y = y * _bubbleSize;
			position.z = 0f;

			var bubbleSlot = Instantiate<BubbleSlotView>(_bubbleSlotPrefab);
			bubbleSlot.transform.SetParent(_bubbleSlotsContainer, false);
			bubbleSlot.transform.localPosition = position;

			_bubbleSlots[i] = new BubbleSlot(bubbleSlot);
		}

		private void CenterGrid()
		{
			_bubbleSlotsContainer.position += Vector3.left * (_bubbleSize * _boardWidth / 2f - _bubbleSize / 4f);
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
			var allRowsButFirst = _bubbleSlots.Skip(6);
			foreach (var bubbleSlot in allRowsButFirst) 
			{
				bubbleSlot.PlaceBubble(bubbleConfigs[random.Next(bubbleConfigs.Count)]);
			}
		}

		private void BubbleAddedToBoard(BubbleSlotView bubbleSlotView, BubbleConfigItem bubbleConfig)
		{
			var bubbleSlot = bubbleSlotView.BubbleSlot();
			bubbleSlot.PlaceBubble(bubbleConfig);  
		}

	}


}
