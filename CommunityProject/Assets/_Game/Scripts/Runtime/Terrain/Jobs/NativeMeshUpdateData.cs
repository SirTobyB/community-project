using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace BoundfoxStudios.CommunityProject.Terrain.Jobs
{
	public struct NativeMeshUpdateData : IDisposable
	{
		public NativeHashMap<byte, UnsafeList<Triangle>> TileTypeTriangles { get; }
		public NativeList<Vertex> Vertices { get; }

		public NativeMeshUpdateData(byte tileTypeCount, Allocator allocator)
		{
			Vertices = new(allocator);

			TileTypeTriangles = new(tileTypeCount, allocator);

			for (var i = 0; i < tileTypeCount; i++)
			{
				TileTypeTriangles.Add((byte)i, new(0, allocator));
			}
		}

		public void AddTriangle(byte tileType, int vertexIndex1, int vertexIndex2, int vertexIndex3)
		{
			var triangleBucket = TileTypeTriangles;
			var triangles = triangleBucket[tileType];
			triangles.Add(new(tileType, vertexIndex1, vertexIndex2, vertexIndex3));
			triangleBucket[tileType] = triangles;
		}

		public void Dispose()
		{
			foreach (var kvp in TileTypeTriangles)
			{
				kvp.Value.Dispose();
			}

			TileTypeTriangles.Dispose();
			Vertices.Dispose();
		}
	}
}
