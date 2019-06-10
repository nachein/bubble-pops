using System;
using System.Collections;
using System.Collections.Generic;
using BubblePops.Shared;
using UnityEngine;

namespace BubblePops.Board 
{
	public class BubbleCatcher : MonoBehaviour
	{
		public event Action<int> OnBubbleDropped = delegate {};

		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.tag == Tags.BUBBLE_DROP)
			{
				var bubbelDropView = other.gameObject.GetComponent<BubbleDropView>();
				OnBubbleDropped(bubbelDropView.BubbleNumber());

				bubbelDropView.Pop();
			}
		}
	}
}
