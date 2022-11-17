using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace BoundfoxStudios.CommunityProject.Terrain.Jobs
{
	public struct NativeMeshUpdateData : IDisposable
	{
		public NativeHashMap<byte, UnsafeList<Triangle>> Triangles { get; }
		public NativeList<Vertex> Vertices { get; }

		public NativeMeshUpdateData(byte tileTypeCount, Allocator allocator)
		{
			Vertices = new(allocator);

			Triangles = new(tileTypeCount, allocator);

			for (var i = 0; i < tileTypeCount; i++)
			{
				Triangles.Add((byte)i, new(0, allocator));
			}
		}

		public void AddToTriangles(byte tileType, int vertexIndex1, int vertexIndex2, int vertexIndex3)
		{
			var triangleBucket = Triangles;
			var triangles = triangleBucket[tileType];
			triangles.Add(new(tileType, vertexIndex1, vertexIndex2, vertexIndex3));
			triangleBucket[tileType] = triangles;
		}

		public void Dispose()
		{
			foreach (var kvp in Triangles)
			{
				kvp.Value.Dispose();
			}

			Triangles.Dispose();
			Vertices.Dispose();
		}
	}
}
