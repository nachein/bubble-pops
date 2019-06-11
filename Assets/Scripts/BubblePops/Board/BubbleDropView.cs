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

		private BubbleConfigItem _bubbleConfig;

		public void SetBubbleConfig (BubbleConfigItem bubbleConfigItem)
		{
			_bubbleConfig = bubbleConfigItem;
			_renderer.color = bubbleConfigItem.color;
			_text.SetText (bubbleConfigItem.display);
		}

		public void Pop ()
		{
			Destroy (gameObject);
		}

		public int BubbleNumber ()
		{
			return _bubbleConfig.number;
		}
	}
}