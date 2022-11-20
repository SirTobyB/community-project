using BoundfoxStudios.CommunityProject.Extensions;
using UnityEngine;
using Grid = BoundfoxStudios.CommunityProject.Terrain.Tiles.Grid;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	[AddComponentMenu(Constants.MenuNames.Terrain + "/" + nameof(TerrainRaycaster))]
	[RequireComponent(typeof(TerrainCollider))]
	public class TerrainRaycaster : MonoBehaviour
	{
		public bool Raycast(Ray ray, out TerrainRaycastHit terrainRaycastHit, float maxRaycastDistance, LayerMask layerMask)
		{
			terrainRaycastHit = default;

			if (!Physics.Raycast(ray, out var hitInfo, maxRaycastDistance, layerMask))
			{
				return false;
			}

			if (!hitInfo.collider.TryGetComponentInParent<TerrainCollider>(out var terrainCollider))
			{
				return false;
			}

			var tilePosition = Grid.WorldToTilePosition(hitInfo.point - terrainCollider.transform.position);

			terrainRaycastHit = new()
			{
				TilePosition = tilePosition
			};

			return true;
		}
	}
}
