using System.Collections;
using System.Collections.Generic;
using BubblePops.Board;
using UnityEngine;

namespace BubblePops.Gameplay 
{
	public class GameplayScene : MonoBehaviour
	{
		[SerializeField] BubbleBoard _bubbleBoard;

		void Awake()
		{
			_bubbleBoard.Create();
		}
	}
}
