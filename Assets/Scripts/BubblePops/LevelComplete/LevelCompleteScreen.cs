using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BubblePops.LevelComplete 
{
	public class LevelCompleteScreen : MonoBehaviour
	{
		public event Action OnRestart = delegate {};

		public void Show()
		{
			gameObject.SetActive(true);
		}

		void Update()
		{
			if (Input.GetMouseButton (0) || Input.touchCount > 0)
			{
				OnRestart();
				Hide();
			}
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}

