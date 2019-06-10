using System;
using System.Collections;
using System.Collections.Generic;
using BubblePops.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace BubblePops.BubbleShoooter
{
    public class BubbleAmmoView : MonoBehaviour
    {
		[SerializeField] SpriteRenderer _renderer;
    	[SerializeField] TextMeshPro _text;

        public void SetBubbleConfig(BubbleConfigItem bubbleConfigItem)
        {
            _renderer.color = bubbleConfigItem.color;
			_text.SetText(bubbleConfigItem.display);
        }
    }
}
