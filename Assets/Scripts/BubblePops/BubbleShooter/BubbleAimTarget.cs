using BubblePops.Board;
using UnityEngine;

namespace BubblePops.BubbleShoooter
{
    public class BubbleAimTarget
	{
		private readonly BubbleSlotView _targetSlot;
		private readonly Vector3 _bouncePosition;
		private bool _bounces;

		public BubbleAimTarget(BubbleSlotView targetSlot)
		{
			_targetSlot = targetSlot;
		}

		public BubbleAimTarget(BubbleSlotView targetSlot, Vector3 bouncePosition)
		{
			_targetSlot = targetSlot;
			_bouncePosition = bouncePosition;
			_bounces = true;
		}

		public bool Bounces()
		{
			return _bounces;
		}

		public Vector3 BouncePosition()
		{
			return _bouncePosition;
		}

		public BubbleSlotView BubbleSlotView()
		{
			return _targetSlot;
		}

	}
}
