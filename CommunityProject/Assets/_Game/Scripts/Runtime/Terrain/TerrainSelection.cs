using BoundfoxStudios.CommunityProject.Terrain.Tiles;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	public struct TerrainSelection
	{
		public IntBounds Bounds { get; set; }
		public Terrain Terrain { get; set; }

		public CardinalDirection? Triangle { get; set; }
	}
}
