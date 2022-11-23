using System;
using BoundfoxStudios.CommunityProject.Infrastructure.SaveManagement;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace BoundfoxStudios.CommunityProject.Terrain.Tiles
{
	public struct TileData : IHaveSaveData<TileData.DataContainer>
	{
		private byte _cornerNorthWestHeight;
		private byte _cornerNorthEastHeight;
		private byte _cornerSouthEastHeight;
		private byte _cornerSouthWestHeight;

		private byte _northTriangleTileTypeId;
		private byte _eastTriangleTileTypeId;
		private byte _southTriangleTileTypeId;
		private byte _westTriangleTileTypeId;

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

		public UniTask<DataContainer> GetDataContainerAsync() => UniTask.FromResult(new DataContainer
		{
			CornerNorthWestHeight = _cornerNorthWestHeight,
			CornerNorthEastHeight = _cornerNorthEastHeight,
			CornerSouthEastHeight = _cornerSouthEastHeight,
			CornerSouthWestHeight = _cornerSouthWestHeight,
			NorthTriangleTileTypeId = _northTriangleTileTypeId,
			EastTriangleTileTypeId = _eastTriangleTileTypeId,
			SouthTriangleTileTypeId = _southTriangleTileTypeId,
			WestTriangleTileTypeId = _westTriangleTileTypeId
		});

		public UniTask SetDataContainerAsync(DataContainer container)
		{
			_cornerNorthWestHeight = container.CornerNorthWestHeight;
			_cornerNorthEastHeight = container.CornerNorthEastHeight;
			_cornerSouthEastHeight = container.CornerSouthEastHeight;
			_cornerSouthWestHeight = container.CornerSouthWestHeight;
			_northTriangleTileTypeId = container.NorthTriangleTileTypeId;
			_eastTriangleTileTypeId = container.EastTriangleTileTypeId;
			_southTriangleTileTypeId = container.SouthTriangleTileTypeId;
			_westTriangleTileTypeId = container.WestTriangleTileTypeId;

			return UniTask.CompletedTask;
		}

		public class DataContainer
		{
			[JsonProperty("ne")]
			public byte CornerNorthEastHeight;

			[JsonProperty("nw")]
			public byte CornerNorthWestHeight;

			[JsonProperty("se")]
			public byte CornerSouthEastHeight;

			[JsonProperty("sw")]
			public byte CornerSouthWestHeight;

			[JsonProperty("e")]
			public byte EastTriangleTileTypeId;

			[JsonProperty("n")]
			public byte NorthTriangleTileTypeId;

			[JsonProperty("s")]
			public byte SouthTriangleTileTypeId;

			[JsonProperty("w")]
			public byte WestTriangleTileTypeId;
		}
	}
}
