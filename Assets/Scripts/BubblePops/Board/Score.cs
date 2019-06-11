using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BubblePops.Board 
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class Score : MonoBehaviour
	{
		private TextMeshProUGUI _scoreText;
		private int _currentScore = 0;

		void Awake()
		{
			_scoreText = GetComponent<TextMeshProUGUI>();
			UpdateCurrentScore();
		}

		void UpdateCurrentScore()
		{
			_scoreText.SetText(_currentScore.ToString());
		}

		public void AddScore(int score)
		{
			_currentScore += score;
			UpdateCurrentScore();
		}

		public void Reset()
		{
			_currentScore = 0;
			UpdateCurrentScore();
		}
	}
}
