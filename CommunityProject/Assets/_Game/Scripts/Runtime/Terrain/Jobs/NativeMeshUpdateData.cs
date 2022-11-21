using System;
using Unity.Collections;

namespace BoundfoxStudios.CommunityProject.Terrain.Jobs
{
	/// <summary>
	///   Data structure to hold all data that is used to generate the terrain mesh.
	/// </summary>
	public readonly struct NativeMeshUpdateData : IDisposable
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
