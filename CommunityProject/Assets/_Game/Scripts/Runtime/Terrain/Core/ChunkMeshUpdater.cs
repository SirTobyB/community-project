using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace BoundfoxStudios.CommunityProject.Terrain.Core
{
	public readonly struct ChunkMeshUpdater : IDisposable
	{
		private readonly Mesh _surfaceMesh;
		private readonly Mesh _wallMesh;

		private readonly Mesh.MeshDataArray _meshDataArray;

		public Mesh.MeshData SurfaceMeshData => _meshDataArray[0];
		public Mesh.MeshData WallMeshData => _meshDataArray[1];

		public ChunkMeshUpdater(Mesh surfaceMesh, Mesh wallMesh)
		{
			_surfaceMesh = surfaceMesh;
			_wallMesh = wallMesh;

			_meshDataArray = Mesh.AllocateWritableMeshData(2);
		}

		public void Dispose()
		{
			// TODO: Directly accessing first data and first submesh may not be a good idea
			var bounds = SurfaceMeshData.GetSubMesh(0).bounds;
			_surfaceMesh.bounds = bounds;

			bounds = WallMeshData.GetSubMesh(0).bounds;
			_wallMesh.bounds = bounds;

			Mesh.ApplyAndDisposeWritableMeshData(_meshDataArray, new[] { _surfaceMesh, _wallMesh },
				MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);

			// TODO: Could be a bit more performant if we do this ourselves in the chunk jobs.
			_surfaceMesh.RecalculateNormals();
			_wallMesh.RecalculateNormals();
		}
	}
}
