using BoundfoxStudios.CommunityProject.Terrain.Jobs;
using BoundfoxStudios.CommunityProject.Terrain.Tiles;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.Tools
{
	public class PreviewMesh
	{
		private readonly float _heightOffset;
		private readonly Mesh _mesh = new();

		public PreviewMesh(float heightOffset)
		{
			_heightOffset = heightOffset;
		}

		public void Render(Matrix4x4 matrix, Material previewMaterial, int layer)
		{
			for (var i = 0; i < _mesh.subMeshCount; i++)
			{
				Graphics.DrawMesh(_mesh, matrix, previewMaterial, layer, null, i);
			}
		}

		public void UpdateMesh(Terrain terrain, IntBounds bounds, Directions? triangleDirection = null)
		{
			var meshDataArray = Mesh.AllocateWritableMeshData(1);
			var meshData = meshDataArray[0];
			var meshUpdateData = new NativeMeshUpdateData(terrain.TileTypes.Length, Allocator.TempJob);

			JobHandle surfaceJobHandle;

			if (triangleDirection is null)
			{
				var surfaceJob = new TerrainSurfaceChunkJob()
				{
					Bounds = bounds,
					Grid = terrain.Grid,
					HeightStep = terrain.HeightStep,
					MeshUpdateData = meshUpdateData,
				};
				surfaceJobHandle = surfaceJob.Schedule();
			}
			else
			{
				var singleTriangleJob = new SingleTriangleJob()
				{
					Grid = terrain.Grid,
					Position = bounds.Center,
					HeightStep = terrain.HeightStep,
					MeshUpdateData = meshUpdateData,
					TriangleDirection = new(triangleDirection.Value)
				};
				surfaceJobHandle = singleTriangleJob.Schedule();
			}

			var normalCalculationJob = new NormalCalculationJob()
			{
				MeshUpdateData = meshUpdateData
			};
			var normalCalculationJobHandle = normalCalculationJob.Schedule(surfaceJobHandle);

			var writeMeshJob = new WriteMeshJob()
			{
				Bounds = new(new(bounds.Center.x, terrain.MaxHeight / 2f, bounds.Center.y),
					new(bounds.Size.x, terrain.MaxHeight, bounds.Size.y)),
				MeshData = meshData,
				MeshUpdateData = meshUpdateData,
				VertexOffset = new(0, _heightOffset, 0)
			};
			var writeChunkMeshJobHandle = writeMeshJob.Schedule(normalCalculationJobHandle);

			writeChunkMeshJobHandle.Complete();

			meshUpdateData.Dispose();
			Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, _mesh);
		}

		public void Clear()
		{
			_mesh.Clear();
		}
	}
}
