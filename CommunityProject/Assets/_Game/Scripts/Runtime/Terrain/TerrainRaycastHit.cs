using BoundfoxStudios.CommunityProject.Terrain.Tiles;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	public struct TerrainRaycastHit
	{
		public Tile Tile { get; set; }
		public Terrain Terrain { get; set; }
	}
}
