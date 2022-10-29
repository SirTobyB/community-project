using System;
using Unity.Collections;

namespace BoundfoxStudios.CommunityProject.Terrain.Core
{
	public struct NativeMeshUpdateData : IDisposable
	{
		public NativeList<Triangle> Triangles { get; }
		public NativeList<Vertex> Vertices { get; }

		public NativeMeshUpdateData(Allocator allocator)
		{
			Triangles = new(allocator);
			Vertices = new(allocator);
		}

		public void Dispose()
		{
			Triangles.Dispose();
			Vertices.Dispose();
		}
	}
}
