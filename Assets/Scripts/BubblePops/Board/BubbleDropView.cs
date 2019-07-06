using System;
using System.Collections;
using System.Collections.Generic;
using BubblePops.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace BubblePops.Board
{
	public class BubbleDropView : MonoBehaviour
	{
		[SerializeField] SpriteRenderer _renderer;
		[SerializeField] TextMeshPro _text;
		[SerializeField] BubblePopView _bubblePop;

		private BubbleConfigItem _bubbleConfig;

		public void SetBubbleConfig (BubbleConfigItem bubbleConfigItem)
		{
			_bubbleConfig = bubbleConfigItem;
			_renderer.color = bubbleConfigItem.color;
			_text.SetText (bubbleConfigItem.display);
		}

        public void Pop()
        {
			SpawnBubblePop();
            Destroy(gameObject);
        }

		private void SpawnBubblePop()
        {
			var bubblePop = Instantiate(_bubblePop, transform.position, Quaternion.identity);
            bubblePop.InstantPop(_bubbleConfig);
        }

        public int BubbleNumber()
		{
			return _bubbleConfig.number;
		}
	}
}