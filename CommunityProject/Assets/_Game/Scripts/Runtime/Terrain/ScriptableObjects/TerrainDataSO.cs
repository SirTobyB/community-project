using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.ScriptableObjects
{
	[CreateAssetMenu(menuName = Constants.MenuNames.Terrain + "/Terrain Data")]
	public class TerrainDataSO : ScriptableObject
	{
		[HideInInspector]
		public string Data;
	}
}
