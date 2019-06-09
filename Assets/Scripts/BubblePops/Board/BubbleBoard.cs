using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BubblePops.ScriptableObjects;
using System;

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

		private BubbleSlot[] _bubbleSlots;
		private float _bubbleSize;

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

			CreateInitialBubbles();
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

		private void CreateInitialBubbles()
		{
			var random = new System.Random();
			var bubbleConfigs = _bubbleConfig.configs;
			var allRowsButFirst = _bubbleSlots.Skip(6);
			foreach (var bubbleSlot in allRowsButFirst) 
			{
				bubbleSlot.PlaceBubble(bubbleConfigs[random.Next(bubbleConfigs.Count)]);
			}
		}
	}

	public class BubbleSlot
	{
        private readonly BubbleSlotView _view;
		private BubbleConfigItem _bubbleConfig;

        public BubbleSlot(BubbleSlotView view)
		{
            _view = view;
        }

        public void PlaceBubble(BubbleConfigItem bubbleConfig)
        {
			_bubbleConfig = bubbleConfig;
        	_view.SetBubbleColor(bubbleConfig.color);
        	_view.SetBubbleNumber(bubbleConfig.display);
        }
    }


}
