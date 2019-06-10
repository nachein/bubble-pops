using System.Collections;
using System.Collections.Generic;
using BubblePops.ScriptableObjects;
using UnityEngine;
using System.Linq;
using System;

namespace BubblePops.BubbleShooter 
{
	public class BubbleAmmo : MonoBehaviour
	{
		[SerializeField] BubbleConfig _bubbleConfig;
		[SerializeField] BubbleAmmoView _bubbleAmmoPrefab;

		private BubbleAmmoView _currentBubbleAmmo;
		private BubbleAmmoView _nextBubbleAmmo;
		private float _nextAmmoScale = 0.7f;
		private float _ammoSize;
		
		void Awake()
		{
			_ammoSize = _bubbleAmmoPrefab.GetComponent<Renderer>().bounds.size.x;
		}

        public void Setup()
		{
			_currentBubbleAmmo = Instantiate(_bubbleAmmoPrefab, CurrentAmmoPosition(), Quaternion.identity);
			_currentBubbleAmmo.SetBubbleConfig(GetRandomAmmoBubbleConfig());

			_nextBubbleAmmo = Instantiate(_bubbleAmmoPrefab, NextAmmoPosition(), Quaternion.identity);
			_nextBubbleAmmo.transform.localScale = Vector3.one * _nextAmmoScale;
			_nextBubbleAmmo.SetBubbleConfig(GetRandomAmmoBubbleConfig());
		}

		public BubbleAmmoView TakeCurrentAmmo()
        {
			var ammoToShoot = _currentBubbleAmmo;

			_currentBubbleAmmo = _nextBubbleAmmo;
			_currentBubbleAmmo.transform.position = transform.position;
			_currentBubbleAmmo.transform.localScale = Vector3.one;

			_nextBubbleAmmo = Instantiate(_bubbleAmmoPrefab, NextAmmoPosition(), Quaternion.identity);
			_nextBubbleAmmo.transform.localScale = Vector3.one * _nextAmmoScale;
			_nextBubbleAmmo.SetBubbleConfig(GetRandomAmmoBubbleConfig());

            return ammoToShoot;
        }

        private BubbleConfigItem GetRandomAmmoBubbleConfig()
		{
			var random = new System.Random(Guid.NewGuid().GetHashCode());
			var bubbleConfigs = _bubbleConfig.configs.Where(config => config.number < 128).ToList();

			return bubbleConfigs[random.Next(bubbleConfigs.Count)];
		}

		private Vector3 CurrentAmmoPosition()
		{
			return transform.position;
		}

		private Vector3 NextAmmoPosition()
        {
            return CurrentAmmoPosition() + Vector3.left * _ammoSize;
        }
		
	}
}
