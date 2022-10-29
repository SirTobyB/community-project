using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.Core
{
	// TODO: Refactor to not inherit from List<Chunk>
	public class Chunks : List<Chunk>
	{
		public Chunks(int2 gridSize, int chunkSize)
		{
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
	}
}
