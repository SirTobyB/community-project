using System;
using Unity.Collections;
using Unity.Mathematics;

namespace BoundfoxStudios.CommunityProject.Terrain.Core
{
	public struct Grid : IDisposable
	{
		/// <summary>
		/// Width of the grid, number of tiles in x direction.
		/// </summary>
		public readonly int Width;

		/// <summary>
		/// Length of the grid, number of tiles in z direction.
		/// </summary>
		public readonly int Length;

		/// <summary>
		/// Height maximum height of the grid
		/// </summary>
		public readonly Height MaxHeight;

		private NativeArray<TileData> _tiles;

		public Grid(int length, int width, Height maxHeight, Allocator allocator)
		{
			Length = length;
			Width = width;
			MaxHeight = maxHeight;
			_tiles = new(Length * Width, allocator);

			GenerateDefaultTiles();
		}

		private void GenerateDefaultTiles()
		{
			var defaultHeight = MaxHeight / 2;

			for (var i = 0; i < _tiles.Length; i++)
			{
				_tiles[0] = new(defaultHeight);
			}
		}

		public readonly Tile GetTile(int2 position) => GetTile(position.x, position.y);

		public readonly Tile GetTile(int x, int z)
		{
			if (x < 0 || x >= Width)
			{
				throw new ArgumentOutOfRangeException(nameof(x));
			}

			if (z < 0 || z >= Length)
			{
				throw new ArgumentOutOfRangeException(nameof(z));
			}

			return new(this, new(x, z));
		}

		internal readonly TileData GetTileData(int2 position) => GetTileData(position.x, position.y);

		internal readonly TileData GetTileData(int x, int z) => _tiles[x + z * Width];

		public void Dispose()
		{
			_tiles.Dispose();
		}
	}
}
