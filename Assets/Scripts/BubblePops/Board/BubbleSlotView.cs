using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BubblePops.Board
{
    public class BubbleSlotView : MonoBehaviour
    {
		[SerializeField] SpriteRenderer _renderer;
    	[SerializeField] TextMeshPro _text;

        public void SetBubbleColor(Color32 color)
        {
            _renderer.color = color;
        }

        public void SetBubbleNumber(string number)
        {
            _text.SetText(number);
        }
    }
}
