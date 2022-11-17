using BoundfoxStudios.CommunityProject.Infrastructure.ScriptableObjects;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.ScriptableObjects
{
	[CreateAssetMenu(menuName = Constants.MenuNames.Terrain + "/Tile Type")]
	public class TileTypeSO : IdentifiableSO
	{
		public Material SurfaceMaterial;
		public Material WallMaterial;
	}
}
