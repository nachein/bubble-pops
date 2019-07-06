using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using BubblePops.ScriptableObjects;

namespace BubblePops.Board
{
	public class BubbleSlotView : MonoBehaviour
	{
		[SerializeField] SpriteRenderer _renderer;
		[SerializeField] TextMeshPro _text;
		[SerializeField] BubblePopView _bubblePop;

		private BubbleSlot _bubbleSlot;

		public void SetBubbleColor (Color32 color)
		{
			_renderer.color = color;
		}

		public void SetBubbleNumber (string number)
		{
			_text.SetText (number);
		}

		public void SetBubbleSlot (BubbleSlot bubbleSlot)
		{
			_bubbleSlot = bubbleSlot;
		}

		public BubbleSlot BubbleSlot ()
		{
			return _bubbleSlot;
		}

		public bool HasBubble ()
		{
			return _bubbleSlot.HasBubble ();
		}

		public bool IsEmpty ()
		{
			return _bubbleSlot.IsEmpty ();
		}

		public void ActivatePreview (BubbleConfigItem config)
		{
			_renderer.color = new Color  (config.color.r/255f, config.color.g/255f, config.color.b/255f, 0.25f);
		}

		public void DeactivatePreview ()
		{
			if (_bubbleSlot.HasBubble())
				return;

			_renderer.color = new Color (_renderer.color.r, _renderer.color.g, _renderer.color.b, 0f);
		}

		public bool IsReserved ()
		{
			return _bubbleSlot.IsReserved ();
		}

		public void Reserve ()
		{
			_bubbleSlot.Reserve ();
		}

		public void Pop(BubbleSlotView mergedBubbleSlot)
		{
			_text.SetText(string.Empty);
			_renderer.color = new Color(1f,1f,1f, 0f);

			if (mergedBubbleSlot != null)
				SpawnBubblePop(mergedBubbleSlot.transform.position);
		}

        private void SpawnBubblePop(Vector3 position)
        {
			var bubblePop = Instantiate(_bubblePop, transform.position, Quaternion.identity);
            bubblePop.Pop(_bubbleSlot.BubbleConfig(), position);
        }
    }
}
