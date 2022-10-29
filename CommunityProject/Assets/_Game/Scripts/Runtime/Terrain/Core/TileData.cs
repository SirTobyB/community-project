using System;

namespace BoundfoxStudios.CommunityProject.Terrain.Core
{
	public struct TileData
	{
		private byte _cornerNorthWestHeight;
		private byte _cornerNorthEastHeight;
		private byte _cornerSouthEastHeight;
		private byte _cornerSouthWestHeight;

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

		public TileData(byte height)
		{
			_cornerNorthWestHeight = height;
			_cornerNorthEastHeight = height;
			_cornerSouthEastHeight = height;
			_cornerSouthWestHeight = height;
		}

		internal TileData(byte cornerNorthWestHeight, byte cornerNorthEastHeight, byte cornerSouthEastHeight,
			byte cornerSouthWestHeight)
		{
			_cornerNorthWestHeight = cornerNorthWestHeight;
			_cornerNorthEastHeight = cornerNorthEastHeight;
			_cornerSouthEastHeight = cornerSouthEastHeight;
			_cornerSouthWestHeight = cornerSouthWestHeight;
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
	}
}
