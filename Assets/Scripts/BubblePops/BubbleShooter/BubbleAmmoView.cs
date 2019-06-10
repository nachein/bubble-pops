using System;
using System.Collections;
using System.Collections.Generic;
using BubblePops.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace BubblePops.BubbleShooter
{
    public class BubbleAmmoView : MonoBehaviour
    {
		[SerializeField] SpriteRenderer _renderer;
    	[SerializeField] TextMeshPro _text;

		private BubbleConfigItem _bubbleConfig;

        public void SetBubbleConfig(BubbleConfigItem bubbleConfigItem)
        {
			_bubbleConfig = bubbleConfigItem;
            _renderer.color = bubbleConfigItem.color;
			_text.SetText(bubbleConfigItem.display);
        }

        public BubbleConfigItem BubbleConfig()
        {
            return _bubbleConfig;
        }
    }
}
