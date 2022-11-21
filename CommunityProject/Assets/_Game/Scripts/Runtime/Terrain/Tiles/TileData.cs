using System;

namespace BoundfoxStudios.CommunityProject.Terrain.Tiles
{
	public struct TileData
	{
		private readonly byte _cornerNorthWestHeight;
		private readonly byte _cornerNorthEastHeight;
		private readonly byte _cornerSouthEastHeight;
		private readonly byte _cornerSouthWestHeight;

		private readonly byte _northTriangleTileTypeId;
		private readonly byte _eastTriangleTileTypeId;
		private readonly byte _southTriangleTileTypeId;
		private readonly byte _westTriangleTileTypeId;

		public bool IsFlat =>
			_cornerNorthWestHeight == _cornerNorthEastHeight
			&& _cornerNorthWestHeight == _cornerSouthEastHeight
			&& _cornerNorthWestHeight == _cornerSouthWestHeight;

		public bool IsSlope => !IsFlat
		                       && (
			                       (_cornerNorthWestHeight == _cornerNorthEastHeight &&
			                        _cornerSouthEastHeight == _cornerSouthWestHeight)
			                       ||
			                       (_cornerNorthWestHeight == _cornerSouthWestHeight &&
			                        _cornerNorthEastHeight == _cornerSouthEastHeight)
		                       );

		public TileData(byte height) : this(height, height, height, height) { }

		internal TileData(
			byte cornerNorthWestHeight,
			byte cornerNorthEastHeight,
			byte cornerSouthEastHeight,
			byte cornerSouthWestHeight,
			byte northTriangleTileTypeId = 0,
			byte eastTriangleTileTypeId = 0,
			byte southTriangleTileTypeId = 0,
			byte westTriangleTileTypeId = 0)
		{
			_cornerNorthWestHeight = cornerNorthWestHeight;
			_cornerNorthEastHeight = cornerNorthEastHeight;
			_cornerSouthEastHeight = cornerSouthEastHeight;
			_cornerSouthWestHeight = cornerSouthWestHeight;
			_northTriangleTileTypeId = northTriangleTileTypeId;
			_eastTriangleTileTypeId = eastTriangleTileTypeId;
			_southTriangleTileTypeId = southTriangleTileTypeId;
			_westTriangleTileTypeId = westTriangleTileTypeId;
		}

		public byte GetHeight(Corner corner) => corner.Direction switch
		{
			CornerDirection.NorthWest => _cornerNorthWestHeight,
			CornerDirection.NorthEast => _cornerNorthEastHeight,
			CornerDirection.SouthEast => _cornerSouthEastHeight,
			CornerDirection.SouthWest => _cornerSouthWestHeight,
			_ => throw new ArgumentOutOfRangeException(nameof(corner), "No valid corner direction")
		};

		public byte GetLowestPoint()
		{
			var lowestPoint = _cornerNorthWestHeight;

			if (_cornerNorthEastHeight < lowestPoint)
			{
				lowestPoint = _cornerNorthEastHeight;
			}

			if (_cornerSouthEastHeight < lowestPoint)
			{
				lowestPoint = _cornerSouthEastHeight;
			}

			if (_cornerSouthWestHeight < lowestPoint)
			{
				lowestPoint = _cornerSouthWestHeight;
			}

			return lowestPoint;
		}

		public int GetCornerCountAtHeight(byte height)
		{
			var count = 0;

			count += _cornerNorthWestHeight == height ? 1 : 0;
			count += _cornerNorthEastHeight == height ? 1 : 0;
			count += _cornerSouthEastHeight == height ? 1 : 0;
			count += _cornerSouthWestHeight == height ? 1 : 0;

			return count;
		}

		public byte GetTileType(Direction direction) => direction.CardinalDirection switch
		{
			CardinalDirection.North => _northTriangleTileTypeId,
			CardinalDirection.East => _eastTriangleTileTypeId,
			CardinalDirection.South => _southTriangleTileTypeId,
			CardinalDirection.West => _westTriangleTileTypeId,
			_ => throw new ArgumentOutOfRangeException(nameof(direction), "Invalid direction")
		};
	}
}
