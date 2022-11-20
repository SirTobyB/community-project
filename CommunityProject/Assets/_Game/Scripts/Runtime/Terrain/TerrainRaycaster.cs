using BoundfoxStudios.CommunityProject.Extensions;
using UnityEngine;
using Grid = BoundfoxStudios.CommunityProject.Terrain.Tiles.Grid;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	[AddComponentMenu(Constants.MenuNames.Terrain + "/" + nameof(TerrainRaycaster))]
	public class TerrainRaycaster : MonoBehaviour
	{
		public bool Raycast(Ray ray, out TerrainRaycastHit terrainRaycastHit, float maxRaycastDistance, LayerMask layerMask)
		{
			terrainRaycastHit = default;

			if (!Physics.Raycast(ray, out var hitInfo, maxRaycastDistance, layerMask))
			{
				return false;
			}

			if (!hitInfo.collider.TryGetComponentInParent<Terrain>(out var terrain))
			{
				return false;
			}

			var tilePosition = Grid.WorldToTilePosition(hitInfo.point - terrain.transform.position);
			var tile = terrain.Grid.GetTile(tilePosition);

			terrainRaycastHit = new()
			{
				Tile = tile,
			};

			return true;
		}
	}
}
