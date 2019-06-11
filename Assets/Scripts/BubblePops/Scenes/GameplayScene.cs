using System.Collections;
using System.Collections.Generic;
using BubblePops.Board;
using UnityEngine;
using UnityEngine.UI;
using BubblePops.LevelComplete;

namespace BubblePops.Gameplay 
{
	public class GameplayScene : MonoBehaviour
	{
		[SerializeField] BubbleBoard _bubbleBoard;
		[SerializeField] LevelCompleteScreen _levelCompleteScreen;

		void Awake()
		{
			_bubbleBoard.Create();
			_bubbleBoard.OnLevelComplete += LevelCompleted;

			_levelCompleteScreen.OnRestart += Restart;
		}

		void Update()
		{
			if (Input.GetKey(KeyCode.E)) 
			{
				_bubbleBoard.LevelCompleted();
			}
		}

		void LevelCompleted()
		{
			_levelCompleteScreen.Show();
		}

		void Restart()
		{
			StartCoroutine(RestartGame());
		}

		IEnumerator RestartGame()
		{
			yield return new WaitForSeconds(0.5f);

			_bubbleBoard.Restart();
		}
	}
}
