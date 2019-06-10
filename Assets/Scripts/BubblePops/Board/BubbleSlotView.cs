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

		private BubbleSlot _bubbleSlot;

        public void SetBubbleColor(Color32 color)
        {
            _renderer.color = color;
        }

        public void SetBubbleNumber(string number)
        {
            _text.SetText(number);
        }
		
		public void SetBubbleSlot(BubbleSlot bubbleSlot)
        {
            _bubbleSlot = bubbleSlot;
        }

		public bool HasBubble()
		{
			return _bubbleSlot.HasBubble();
		}

		public bool IsEmpty()
		{
			return _bubbleSlot.IsEmpty();
		}

		public void ActivatePreview()
		{
			_renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 1f);
		}
		
		public void DeactivatePreview()
		{
			_renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 0f);
		}
    }
}
