using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.Core
{
	public class Chunks : List<Chunk>
	{
		/*private readonly HashSet<int2> _dirtyPositions;
		private readonly HashSet<IntBounds> _dirtyBounds;
		private readonly int2 _chunkSize;*/
		// private readonly IntBounds _worldBounds;

		// public bool HasAny => _dirtyPositions.Count > 0 || _dirtyBounds.Count > 0;

		public Chunks(int2 gridSize, int chunkSize)
		{
		//	_worldBounds = new(new(0), gridSize);
			GenerateChunks(gridSize, chunkSize);

		}

		private void GenerateChunks(int2 gridSize, int chunkSize)
		{
			var worldBounds = new IntBounds(new(0), gridSize);
			var chunkBounds = new IntBounds(0, chunkSize);

			var numberOfChunks = worldBounds.Size / chunkSize;

			Capacity = numberOfChunks.x * numberOfChunks.y;

			for (var x = 0; x < numberOfChunks.x; x++)
			{
				for (var z = 0; z < numberOfChunks.y; z++)
				{
					var position = new int2(x * chunkSize, z * chunkSize);

					Add(new(position, chunkBounds));
				}
			}
		}

		/*private bool IsDirty(IntBounds bounds) => _dirtyBounds.Any(b => b.Intersects(bounds)) || _dirtyPositions.Any(bounds.Contains);

		public IEnumerable<int2> GetDirtyPositions()
		{


			for (var x = 0; x < numberOfChunks.x; x++)
			{
				for (var z = 0; z < numberOfChunks.y; z++)
				{
					var position = new int2(x, z);

					var chunkBounds = GetChunkBounds(position);

					if (IsDirty(chunkBounds))
					{
						yield return position;
					}
				}
			}
		}

		private IntBounds GetChunkBounds(int2 position) => _worldBounds.Intersection(position * _chunkSize);*/
	}
}
