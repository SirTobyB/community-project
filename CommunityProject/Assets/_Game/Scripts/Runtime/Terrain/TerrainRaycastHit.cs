using BoundfoxStudios.CommunityProject.Terrain.Tiles;
using Unity.Mathematics;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	public struct TerrainRaycastHit
	{
		/// <summary>
		/// The tile that was hit.
		/// </summary>
		public Tile Tile { get; set; }

		/// <summary>
		/// The Terrain the <see cref="Tile"/> belongs to.
		/// </summary>
		public Terrain Terrain { get; set; }

		/// <summary>
		/// The actual triangle in the tile that was hit.
		/// </summary>
		public Directions? TriangleDirection { get; set; }

		public bool IsWall => math.distance(math.dot(Normal, math.up()), 0) < float.Epsilon;
		public float3 Normal { get; set; }
	}
}
