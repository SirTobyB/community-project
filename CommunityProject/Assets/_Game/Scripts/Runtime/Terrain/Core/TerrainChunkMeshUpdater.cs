using System;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.Core
{
	public readonly struct TerrainChunkMeshUpdater : IDisposable
	{
		private readonly Mesh _mesh;

		private readonly Mesh.MeshDataArray _meshDataArray;
		public Mesh.MeshData MeshData => _meshDataArray[0];

		public TerrainChunkMeshUpdater(Mesh mesh)
		{
			_mesh = mesh;
			_meshDataArray = Mesh.AllocateWritableMeshData(1);
		}

		public void Dispose()
		{
			// TODO: Directly accessing first data and first submesh may not be a good idea
			var bounds = _meshDataArray[0].GetSubMesh(0).bounds;
			_mesh.bounds = bounds;
			Mesh.ApplyAndDisposeWritableMeshData(_meshDataArray, _mesh);
		}
	}
}
