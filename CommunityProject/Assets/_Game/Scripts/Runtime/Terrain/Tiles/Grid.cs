using System;
using System.Collections.Generic;
using BoundfoxStudios.CommunityProject.Infrastructure.SaveManagement;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Mathematics;

namespace BoundfoxStudios.CommunityProject.Terrain.Tiles
{
	public struct Grid : IDisposable, IHaveSaveData<Grid.DataContainer>
	{
		private readonly Allocator _allocator;

		/// <summary>
		/// Width of the grid, number of tiles in x direction.
		/// </summary>
		public int Width { get; private set; }

		/// <summary>
		/// Length of the grid, number of tiles in z direction.
		/// </summary>
		public int Length { get; private set; }

		/// <summary>
		/// Height maximum height of the grid
		/// </summary>
		public byte MaxHeight { get; private set; }

		private NativeArray<TileData> _tiles;
		private IntBounds _bounds;

		public Grid(int width, int length, byte maxHeight, Allocator allocator)
		{
			_allocator = allocator;

			Width = width;
			Length = length;
			MaxHeight = maxHeight;
			_tiles = new(Width * Length, allocator);
			_bounds = new(int2.zero, new(Width, Length));

			GenerateDefaultTiles();
		}

		private void GenerateDefaultTiles()
		{
			var defaultHeight = (byte)(MaxHeight / 2);

			for (var i = 0; i < _tiles.Length; i++)
			{
				_tiles[i] = new(defaultHeight);
			}
		}

		public readonly Tile GetTile(int2 position) => GetTile(position.x, position.y);

		public readonly Tile GetTile(int x, int z)
		{
			if (x < 0 || x >= Width)
			{
				throw new ArgumentOutOfRangeException(nameof(x), $"{nameof(x)} {x} is less than 0 or greater than {Length}");
			}

			if (z < 0 || z >= Length)
			{
				throw new ArgumentOutOfRangeException(nameof(z), $"{nameof(z)} {z} is less than 0 or greater than {Length}");
			}

			return new(this, new(x, z));
		}

		internal readonly TileData GetTileData(int2 position)
		{
			var defaultHeight = (byte)(MaxHeight / 2);

			// TODO: Remove test data
			/*if (position.Equals(new(1, 1)))
			{
				return new((byte)(defaultHeight + 1), (byte)(defaultHeight + 1), defaultHeight, defaultHeight, 1, 2, 1, 1);
			}

			if (position.Equals(new(3, 1)))
			{
				return new((byte)(defaultHeight + 0), (byte)(defaultHeight + 0), (byte)(defaultHeight + 1),
					(byte)(defaultHeight + 0), 1);
			}

			if (position.Equals(new(5, 1)))
			{
				return new((byte)(defaultHeight + 0), (byte)(defaultHeight + 0), (byte)(defaultHeight + 1),
					(byte)(defaultHeight + 0), 0, 1);
			}

			if (position.Equals(new(6, 0)))
			{
				return new((byte)(defaultHeight + 1), (byte)(defaultHeight + 0), (byte)(defaultHeight + 1),
					(byte)(defaultHeight + 0), 0, 0, 1);
			}

			if (position.Equals(new(2, 1)))
			{
				return new((byte)(defaultHeight + 2), (byte)(defaultHeight + 2), (byte)(defaultHeight + 1),
					(byte)(defaultHeight + 0), 0, 0, 0, 1);
			}

			if (position.Equals(new(2, 2)))
			{
				return new((byte)(defaultHeight + 2), (byte)(defaultHeight + 2), (byte)(defaultHeight + 1),
					(byte)(defaultHeight + 0));
			}*/

			return GetTileData(position.x, position.y);
		}

		internal readonly TileData GetTileData(int x, int z) => _tiles[x + z * Width];

		public void Dispose()
		{
			if (_tiles.IsCreated)
			{
				_tiles.Dispose();
			}
		}

		public readonly bool IsInBounds(int2 position) => _bounds.Contains(position);

		/// <summary>
		/// Since the grid does not have any information about a local position, it always assumes
		/// that the worldPosition is from origin.
		/// </summary>
		public static int2 WorldToTilePosition(float3 worldPosition) => new(worldPosition.xz);

		public class DataContainer
		{
			[JsonProperty("w")]
			public int Width;

			[JsonProperty("l")]
			public int Length;

			[JsonProperty("h")]
			public byte MaxHeight;

			[JsonProperty("t")]
			public List<TileData.DataContainer> Tiles;
		}

		public async UniTask<DataContainer> GetDataContainerAsync()
		{
			var container = new DataContainer()
			{
				Width = Width,
				Length = Length,
				MaxHeight = MaxHeight,
				Tiles = new()
			};

			foreach (var tile in _tiles)
			{
				container.Tiles.Add(await tile.GetDataContainerAsync());
			}

			return container;
		}

		public async UniTask SetDataContainerAsync(DataContainer container)
		{
			Width = container.Width;
			Length = container.Length;
			MaxHeight = container.MaxHeight;

			var tileSize = Width * Length;

			if (tileSize != _tiles.Length)
			{
				if (_tiles.IsCreated)
				{
					_tiles.Dispose();
				}

				_tiles = new(tileSize, _allocator);
			}

			for (var index = 0; index < container.Tiles.Count; index++)
			{
				var tileContainer = container.Tiles[index];
				var tileData = new TileData();
				await tileData.SetDataContainerAsync(tileContainer);

				_tiles[index] = tileData;
			}

			_bounds = new(int2.zero, new(Width, Length));
		}
	}
}
