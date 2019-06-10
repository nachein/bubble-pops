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
		private BubbleAmmoView _bubbleAmmoToShoot;

		void Awake()
		{
			_bubbleAmmo = GetComponent<BubbleAmmo>();
		}

		public void Shoot(BubbleAimTarget aimTarget)
		{
			_bubbleAmmoToShoot = _bubbleAmmo.TakeCurrentAmmo();
			StartCoroutine(ShootBubble(aimTarget));
		}

		private IEnumerator ShootBubble(BubbleAimTarget aimTarget)
		{
			if (aimTarget.Bounces())
				yield return StartCoroutine(MoveTo(aimTarget.BouncePosition()));

			yield return StartCoroutine(MoveTo(aimTarget.BubbleSlotView().transform.position));

			OnBubbleAdded(aimTarget.BubbleSlotView(), _bubbleAmmoToShoot.BubbleConfig());

			yield return null;

			Destroy(_bubbleAmmoToShoot.gameObject);
		}

		private IEnumerator MoveTo(Vector3 destination)
		{
			while((_bubbleAmmoToShoot.transform.position - destination).sqrMagnitude > 0.0001f)
			{
				_bubbleAmmoToShoot.transform.position = Vector3.MoveTowards(_bubbleAmmoToShoot.transform.position, destination, 1.0f * 0.5f);
				yield return null;
			}
		}
	}
}
