using Unity.Mathematics;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.Core
{
	public struct Tile
	{
		internal static readonly float3[] CornerOffsetsFromCenter = {
			new(-0.5f, 0, 0.5f), // North West
			new(0.5f, 0, 0.5f),  // North East
			new(0.5f, 0, -0.5f), // South East
			new(-0.5f, 0, -0.5f) // South West
		};

		private readonly Grid _grid;

		public int2 Position { get; }
		public float3 BottomCenter { get; }

		public float Center => GetCenter(GetData());

		public Tile(Grid grid, int2 position)
		{
			_grid = grid;
			Position = position;
			BottomCenter = new(Position.x + 0.5f, 0, Position.y + 0.5f);
		}

		internal readonly TileData GetData() => _grid.GetTileData(Position);

		internal float GetCenter(TileData data)
		{
			var lowestPoint = data.GetLowestPoint();
			float center = lowestPoint;

			// One corner is below all other corners
			if (data.GetCornerCountAtHeight(lowestPoint) == 1)
			{
				center++;
			}

			if (data.IsSlope)
			{
				center += 0.5f;
			}

			return center;
		}
	}
}

