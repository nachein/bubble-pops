using System.Collections;
using System.Collections.Generic;
using BubblePops.Board;
using UnityEngine;
using UnityEngine.UI;

namespace BubblePops.Gameplay 
{
	public class GameplayScene : MonoBehaviour
	{
		[SerializeField] BubbleBoard _bubbleBoard;
		[SerializeField] Button _restartButton;

		void Awake()
		{
			_bubbleBoard.Create();

			_restartButton.onClick.AddListener(Restart);
		}

		void Restart()
		{
			_bubbleBoard.Restart();
		}
	}
}
