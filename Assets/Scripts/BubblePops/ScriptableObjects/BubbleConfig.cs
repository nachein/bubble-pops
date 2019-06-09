using System.Collections.Generic;
using UnityEngine;

namespace BubblePops.ScriptableObjects
{
	[CreateAssetMenu(fileName = "BubbleConfig", menuName = "ScriptableObjects/BubbleConfig", order = 1)]
	public class BubbleConfig : ScriptableObject
	{
		public List<BubbleConfigItem> configs;
	}	
}
