using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace BoundfoxStudios.CommunityProject.Terrain.Chunks
{
	public readonly struct ChunkMeshUpdater : IDisposable
	{
		private readonly Mesh _surfaceMesh;
		private readonly Mesh _wallMesh;
		private readonly Bounds _subMeshBounds;

		private readonly Mesh.MeshDataArray _meshDataArray;

		public Mesh.MeshData SurfaceMeshData => _meshDataArray[0];
		public Mesh.MeshData WallMeshData => _meshDataArray[1];

		public ChunkMeshUpdater(Mesh surfaceMesh, Mesh wallMesh, Bounds subMeshBounds)
		{
			_surfaceMesh = surfaceMesh;
			_wallMesh = wallMesh;
			_subMeshBounds = subMeshBounds;

			_meshDataArray = Mesh.AllocateWritableMeshData(2);
		}

		public void Dispose()
		{
			_surfaceMesh.bounds = _subMeshBounds;
			_wallMesh.bounds = _subMeshBounds;

			Mesh.ApplyAndDisposeWritableMeshData(_meshDataArray, new[] { _surfaceMesh, _wallMesh },
				MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);
		}
	}
}
