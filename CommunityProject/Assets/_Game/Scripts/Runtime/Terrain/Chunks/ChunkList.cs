using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BoundfoxStudios.CommunityProject.Terrain.Tiles;
using Unity.Mathematics;

namespace BoundfoxStudios.CommunityProject.Terrain.Chunks
{
	public class ChunkList : IEnumerable<Chunk>
	{
		private readonly List<Chunk> _chunks = new();

		public ReadOnlyCollection<Chunk> Chunks => new(_chunks);

		public ChunkList(int2 gridSize, int chunkSize, byte maxHeight)
		{
			GenerateChunks(gridSize, chunkSize, maxHeight);
		}

		private void GenerateChunks(int2 gridSize, int chunkSize, byte maxHeight)
		{
			var worldBounds = new IntBounds(new(0), gridSize);
			var numberOfChunks = worldBounds.Size / chunkSize;

			_chunks.Capacity = numberOfChunks.x * numberOfChunks.y;

			for (var x = 0; x < numberOfChunks.x; x++)
			{
				for (var z = 0; z < numberOfChunks.y; z++)
				{
					var position = new int2(x * chunkSize, z * chunkSize);

					_chunks.Add(new(new(position, position + chunkSize), maxHeight));
				}
			}
		}

		public IEnumerator<Chunk> GetEnumerator() => Chunks.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
