using Unity.Mathematics;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.Core
{
	public struct Tile
	{
		internal static readonly float3[] CornerOffsetsFromCenter =
		{
			new(-0.5f, 0, 0.5f), // North West
			new(0.5f, 0, 0.5f), // North East
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

		public Tile GetNeighbor(Direction direction)
		{
			var offset = direction.ToVector();
			var neighborPosition = Position + offset;

			if (!_grid.IsInBounds(neighborPosition))
			{
				// This will create an invalid tile which is not part of the grid.
				return new(_grid, neighborPosition);
			}

			return _grid.GetTile(neighborPosition);
		}

		public float3 GetCornerPosition(Corner corner)
		{
			return BottomCenter + GetCornerOffset(corner);
		}

		public float3 GetCornerOffset(Corner corner)
		{
			if (!IsInBounds)
			{
				return CornerOffsetsFromCenter[corner.Index];
			}

			var height = GetData().GetHeight(corner);
			return CornerOffsetsFromCenter[corner.Index] + new float3(0, height, 0);
		}

		public readonly bool IsInBounds => _grid.IsInBounds(Position);
		public readonly byte GetHeight(Corner corner) => GetData().GetHeight(corner);
	}
}

