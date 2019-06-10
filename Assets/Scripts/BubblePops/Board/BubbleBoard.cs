﻿using System.Collections;
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

			var bubbleSlot = Instantiate<BubbleSlotView>(_bubbleSlotPrefab);
			bubbleSlot.transform.SetParent(_bubbleSlotsContainer, false);
			bubbleSlot.transform.localPosition = position;

			_bubbleSlots[i] = new BubbleSlot(bubbleSlot);
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

			if (_bubbleSlots.Skip(Math.Max(0, _bubbleSlots.Length - _boardWidth)).Any(slot => slot.HasBubble()))
			{
				AddNewRow();
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
	}


}
