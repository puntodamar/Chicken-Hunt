using UnityEngine;

namespace Scriptables
{
	[CreateAssetMenu(fileName = "Resources/LevelDatas/LevelData", menuName = "LevelData/Level Data", order = 1)]
	public class LevelData : ScriptableObject
	{
		public int TotalChickenToEat;
	}
}
