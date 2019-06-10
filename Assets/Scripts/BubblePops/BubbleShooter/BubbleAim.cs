using System;
using System.Collections;
using System.Collections.Generic;
using BubblePops.Board;
using BubblePops.Shared;
using UnityEngine;

namespace BubblePops.BubbleShooter
{
	[RequireComponent(typeof(BubbleShoot))]
    public class BubbleAim : MonoBehaviour 
	{
		[SerializeField] LineRenderer aimLineRenderer;

		public event Action<BubbleSlotView> OnBubbleSlotPreviewActivated = delegate {};
		public event Action OnNoBubbleSlotPreviewActivated = delegate {};
		public event Action OnBubbleShot = delegate {};

		private BubbleShoot _bubbleShoot;
		private BubbleAimTarget _aimTarget;

		void Awake()
		{
			_bubbleShoot = GetComponent<BubbleShoot>();
		}

		void Start()
		{
			aimLineRenderer.positionCount = 1;
			aimLineRenderer.SetPositions(new Vector3[] { transform.position });
		}

		void Update ()
		{
			if (IsAiming()) 
			{
				aimLineRenderer.positionCount = 3;

				_aimTarget = null;
				HandleAim();
			}
			else if (FinishedAiming())
			{
				if (_aimTarget != null) 
				{
					_aimTarget.BubbleSlotView().Reserve();
					OnBubbleShot();
					_bubbleShoot.Shoot(_aimTarget);
				}
					
			}
			else
			{
				aimLineRenderer.positionCount = 1;
				OnNoBubbleSlotPreviewActivated();
			}
		}

		private bool IsAiming()
		{
			return Input.GetMouseButton(0) || Input.touchCount > 0 && Input.touches[0].phase != TouchPhase.Ended;
		}

		private bool FinishedAiming()
		{
			return Input.GetMouseButtonUp(0) || Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended;
		}

		private void HandleAim()
		{
			var touchPosition = AimingPosition();

			var aimHits = Physics2D.RaycastAll(transform.position, touchPosition - transform.position);

			for (var i = 0; i < aimHits.Length; i++) 
			{
				var aimHit = aimHits[i];

				if (IsAimingAtEmptySlot(aimHit))
					continue;

				if (IsAimingAtAvailableBubble(aimHit))
				{
					aimLineRenderer.SetPosition(1, aimHit.point);
					aimLineRenderer.SetPosition(2, aimHit.point);
					if (i > 0) 
					{
						var targetSlot = aimHits[i-1].transform.GetComponent<BubbleSlotView>();
						OnBubbleSlotPreviewActivated(targetSlot);

						_aimTarget = new BubbleAimTarget(targetSlot);
					}
						
					break;
				}

				if (IsAimingAtWall(aimHit))
				{
					aimLineRenderer.SetPosition(1, aimHit.point);

					var reflectionDirection = Vector3.Reflect(touchPosition - transform.position, aimHit.normal);

					var reflectionHits = Physics2D.RaycastAll(aimHit.point, reflectionDirection, 100, ~(1 << LayerMask.NameToLayer (Layers.BOUNCE_SIDE)));

					for (var j = 0; j < reflectionHits.Length; j++) 
					{
						var reflectionHit = reflectionHits[j];

						if (IsAimingAtEmptySlot(reflectionHit))
						{
							continue;
						}
							
						if (IsAimingAtAvailableBubble(reflectionHit))
						{
							aimLineRenderer.SetPosition(2, reflectionHit.point);
							if (j > 0) 
							{
								var targetSlot = reflectionHits[j-1].transform.GetComponent<BubbleSlotView>();
								OnBubbleSlotPreviewActivated(targetSlot);

								_aimTarget = new BubbleAimTarget(targetSlot, aimHit.point);
							}
								
							else if (i > 0) 
							{
								var targetSlot = aimHits[i-1].transform.GetComponent<BubbleSlotView>();
								OnBubbleSlotPreviewActivated(targetSlot);

								_aimTarget = new BubbleAimTarget(targetSlot, aimHit.point);
							}

							break;
						}
					}

					break;
				}
			}
		}

		private Vector3 AimingPosition()
		{
			if (Input.touchCount > 0)
				return Camera.main.ScreenToWorldPoint(Input.touches[0].position);

			return Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}

		private bool IsAimingAtAvailableBubble(RaycastHit2D hit)
		{
			if (hit.transform.gameObject.tag == Tags.BUBBLE_SLOT) 
			{
				var bubbleSlotView = hit.transform.GetComponent<BubbleSlotView>();
				return bubbleSlotView.HasBubble() || bubbleSlotView.IsReserved();
			}

			return false;
		}

		private bool IsAimingAtEmptySlot(RaycastHit2D hit)
		{
			if (hit.transform.gameObject.tag == Tags.BUBBLE_SLOT) 
			{
				var bubbleSlotView = hit.transform.GetComponent<BubbleSlotView>();
				return bubbleSlotView.IsEmpty() && !bubbleSlotView.IsReserved();
			}

			return false;
		}

		private bool IsAimingAtWall(RaycastHit2D hit)
		{
			return hit.transform.gameObject.tag == Tags.BOUNCE_SIDE;
		}
	}
}
