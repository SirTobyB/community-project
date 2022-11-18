using System;
using Unity.Collections;

namespace BoundfoxStudios.CommunityProject.Terrain.Jobs
{
	public struct NativeMeshUpdateData : IDisposable
	{
		public NativeList<Triangle> Triangles { get; }
		public NativeList<Vertex> Vertices { get; }
		public byte SubMeshCount { get; }

		public NativeMeshUpdateData(byte tileTypeCount, Allocator allocator)
		{
			Vertices = new(allocator);
			Triangles = new(allocator);
			SubMeshCount = tileTypeCount;
		}

		public void Dispose()
		{
			Triangles.Dispose();
			Vertices.Dispose();
		}
	}
}
