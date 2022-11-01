using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

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
		public readonly byte MaxHeight;

		private NativeArray<TileData> _tiles;
		private IntBounds _bounds;

		public IntBounds B => _bounds;

		public Grid(int width, int length, byte maxHeight, Allocator allocator)
		{
			Width = width;
			Length = length;
			MaxHeight = maxHeight;
			_tiles = new(Width * Length, allocator);
			_bounds = new(int2.zero, new(Width, Length));

			GenerateDefaultTiles();
		}

		private void GenerateDefaultTiles()
		{
			var defaultHeight = (byte) (MaxHeight / 2);

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
			var defaultHeight = (byte) (MaxHeight / 2);

			if (position.Equals(new(1,1)))
			{
				return new((byte) (defaultHeight + 1), (byte)(defaultHeight + 1), defaultHeight, defaultHeight);
			}

			if (position.Equals(new(3,1)))
			{
				return new((byte) (defaultHeight + 0), (byte)(defaultHeight + 0), (byte) (defaultHeight + 1), (byte) (defaultHeight + 0));
			}

			if (position.Equals(new(5,1)))
			{
				return new((byte) (defaultHeight + 0), (byte)(defaultHeight + 0), (byte) (defaultHeight + 1), (byte) (defaultHeight + 0));
			}

			if (position.Equals(new(6,0)))
			{
				return new((byte) (defaultHeight + 1), (byte)(defaultHeight + 0), (byte) (defaultHeight + 1), (byte) (defaultHeight + 0));
			}

			if (position.Equals(new(2, 1)))
			{
				return new((byte)(defaultHeight + 2), (byte)(defaultHeight + 2), (byte)(defaultHeight + 1), (byte)(defaultHeight + 0));
			}

			if (position.Equals(new(2, 2)))
			{
				return new((byte)(defaultHeight + 2), (byte)(defaultHeight + 2), (byte)(defaultHeight + 1), (byte)(defaultHeight + 0));
			}

			return GetTileData(position.x, position.y);
		}

		internal readonly TileData GetTileData(int x, int z) => _tiles[x + z * Width];

		public void Dispose()
		{
			_tiles.Dispose();
		}

		public readonly bool IsInBounds(int2 position) => _bounds.Contains(position);
	}
}
