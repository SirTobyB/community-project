using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BoundfoxStudios.CommunityProject.Terrain.Tiles;
using Unity.Mathematics;

namespace BoundfoxStudios.CommunityProject.Terrain.Chunks
{
	// TODO: Refactor to not inherit from List<Chunk>
	public class ChunkList : IEnumerable<Chunk>
	{
		private readonly List<Chunk> _chunks = new();

		public ReadOnlyCollection<Chunk> Chunks => new(_chunks);

		public ChunkList(int2 gridSize, int chunkSize)
		{
			GenerateChunks(gridSize, chunkSize);
		}

		private void GenerateChunks(int2 gridSize, int chunkSize)
		{
			var worldBounds = new IntBounds(new(0), gridSize);
			var chunkBounds = new IntBounds(0, chunkSize);

			var numberOfChunks = worldBounds.Size / chunkSize;

			_chunks.Capacity = numberOfChunks.x * numberOfChunks.y;

			for (var x = 0; x < numberOfChunks.x; x++)
			{
				for (var z = 0; z < numberOfChunks.y; z++)
				{
					var position = new int2(x * chunkSize, z * chunkSize);

					_chunks.Add(new(position, chunkBounds));
				}
			}
		}

		public IEnumerator<Chunk> GetEnumerator() => Chunks.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
