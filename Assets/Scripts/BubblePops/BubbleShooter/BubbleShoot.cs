using System;
using System.Collections;
using System.Collections.Generic;
using BubblePops.Board;
using BubblePops.ScriptableObjects;
using UnityEngine;

namespace BubblePops.BubbleShooter 
{
	[RequireComponent(typeof(BubbleAmmo))]
	public class BubbleShoot : MonoBehaviour
	{
		public event Action<BubbleSlotView, BubbleConfigItem> OnBubbleAdded = delegate {};

		private BubbleAmmo _bubbleAmmo;

		void Awake()
		{
			_bubbleAmmo = GetComponent<BubbleAmmo>();
		}

		public void Shoot(BubbleAimTarget aimTarget)
		{
			StartCoroutine(ShootBubble(aimTarget, _bubbleAmmo.TakeCurrentAmmo()));
		}

		private IEnumerator ShootBubble(BubbleAimTarget aimTarget, BubbleAmmoView ammoView)
		{
			if (aimTarget.Bounces())
				yield return StartCoroutine(MoveTo(aimTarget.BouncePosition(), ammoView));

			yield return StartCoroutine(MoveTo(aimTarget.BubbleSlotView().transform.position, ammoView));

			OnBubbleAdded(aimTarget.BubbleSlotView(), ammoView.BubbleConfig());

			yield return null;

			Destroy(ammoView.gameObject);
		}

		private IEnumerator MoveTo(Vector3 destination, BubbleAmmoView ammoView)
		{
			while((ammoView.transform.position - destination).sqrMagnitude > 0.0001f)
			{
				ammoView.transform.position = Vector3.MoveTowards(ammoView.transform.position, destination, 1.0f * 0.5f);
				yield return null;
			}
		}
	}
}
