using System;
using System.Collections.Generic;
using BoundfoxStudios.CommunityProject.Terrain.Chunks;
using BoundfoxStudios.CommunityProject.Terrain.Jobs;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Profiling;
using Grid = BoundfoxStudios.CommunityProject.Terrain.Tiles.Grid;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	[AddComponentMenu(Constants.MenuNames.Terrain + "/" + nameof(TerrainRenderer))]
	[RequireComponent(typeof(Terrain))]
	[ExecuteAlways]
	public class TerrainRenderer : MonoBehaviour
	{
		private readonly List<Chunk> _chunkCache = new();
		private readonly List<Chunk> _chunksToUpdate = new();

		private Terrain _terrain;

		private void Awake()
		{
			_terrain = GetComponent<Terrain>();
		}

		private void LateUpdate()
		{
			UpdateDirtyChunks();
			RenderMeshes();
		}

		private void OnEnable()
		{
			_terrain.UpdateChunks += MarkChunksDirty;
		}

		private void OnDisable()
		{
			_terrain.UpdateChunks -= MarkChunksDirty;
		}

		public event Action<IReadOnlyCollection<Chunk>> AfterUpdateChunks = delegate { };

		private void MarkChunksDirty(IReadOnlyCollection<Chunk> chunks)
		{
			_chunksToUpdate.AddRange(chunks);
		}

		private void UpdateDirtyChunks()
		{
			if (_chunksToUpdate.Count == 0)
			{
				return;
			}

			UpdateChunks(_chunksToUpdate);
			_chunksToUpdate.Clear();
		}

		private void UpdateChunks(List<Chunk> chunksToUpdate)
		{
			Profiler.BeginSample("Updating ChunkList");

			var chunkJobPairs = new List<ChunkJobPair>();

			foreach (var chunk in chunksToUpdate)
			{
				var jobPair = new ChunkJobPair
				{
					Chunk = chunk,
					Grid = _terrain.Grid,
					MaxHeight = _terrain.MaxHeight,
					HeightStep = _terrain.HeightStep,
					TileTypeCount = _terrain.TileTypes.Length
				};

				jobPair.Schedule();

				chunkJobPairs.Add(jobPair);
			}

			JobHandle.ScheduleBatchedJobs();

			foreach (var pair in chunkJobPairs)
			{
				if (!_chunkCache.Contains(pair.Chunk))
				{
					_chunkCache.Add(pair.Chunk);
				}

				pair.Complete();
				pair.Dispose();
			}

			Profiler.EndSample();

			AfterUpdateChunks(chunksToUpdate);
		}

		private void RenderMeshes()
		{
			foreach (var chunk in _chunkCache)
			{
				var matrix = transform.localToWorldMatrix;
				chunk.Render(matrix, _terrain.TileTypes, gameObject.layer);
			}
		}

		private struct ChunkJobPair : IDisposable
		{
			private ChunkMeshUpdater _meshUpdater;
			private JobHandle _jobHandle;
			private NativeMeshUpdateData _surfaceMeshUpdateData;
			private NativeMeshUpdateData _wallMeshUpdateData;

			public Chunk Chunk { get; set; }
			public Grid Grid { get; set; }
			public byte MaxHeight { get; set; }
			public float HeightStep { get; set; }
			public byte TileTypeCount { get; set; }

			public void Schedule()
			{
				_meshUpdater = Chunk.AcquireMeshUpdater();
				_surfaceMeshUpdateData = new(TileTypeCount, Allocator.Persistent);
				_wallMeshUpdateData = new(TileTypeCount, Allocator.Persistent);

				var surfaceUpdateJobHandle = ScheduleSurfaceUpdate(Chunk.SubMeshBounds);
				var wallUpdateJobHandle = ScheduleWallUpdate(Chunk.SubMeshBounds);

				_jobHandle = JobHandle.CombineDependencies(
					surfaceUpdateJobHandle,
					wallUpdateJobHandle
				);
			}

			private JobHandle ScheduleSurfaceUpdate(Bounds bounds)
			{
				var surfaceChunkJob = new TerrainSurfaceChunkJob
				{
					MeshUpdateData = _surfaceMeshUpdateData,
					Grid = Grid,
					Bounds = Chunk.Bounds,
					HeightStep = HeightStep
				};
				var surfaceChunkJobHandle = surfaceChunkJob.Schedule();

				var surfaceNormalCalculationJob = new NormalCalculationJob
				{
					MeshUpdateData = _surfaceMeshUpdateData
				};
				var surfaceNormalCalculationJobHandle = surfaceNormalCalculationJob.Schedule(surfaceChunkJobHandle);

				var combineNormalsJob = new CombineNormalsJob
				{
					SurfaceMeshUpdateData = _surfaceMeshUpdateData
				};
				var combineNormalsJobHandle = combineNormalsJob.Schedule(surfaceNormalCalculationJobHandle);

				var meshWriteJob = new WriteMeshJob
				{
					Bounds = bounds,
					MeshData = _meshUpdater.SurfaceMeshData,
					MeshUpdateData = _surfaceMeshUpdateData
				};

				return meshWriteJob.Schedule(combineNormalsJobHandle);
			}

			private JobHandle ScheduleWallUpdate(Bounds bounds)
			{
				var wallChunkJob = new TerrainWallChunkJob
				{
					MeshUpdateData = _wallMeshUpdateData,
					Grid = Grid,
					Bounds = Chunk.Bounds,
					HeightStep = HeightStep
				};
				var wallChunkJobHandle = wallChunkJob.Schedule();

				var wallNormalCalculationJob = new NormalCalculationJob
				{
					MeshUpdateData = _wallMeshUpdateData
				};
				var wallNormalCalculationJobHandle = wallNormalCalculationJob.Schedule(wallChunkJobHandle);

				var meshWriteJob = new WriteMeshJob
				{
					Bounds = bounds,
					MeshData = _meshUpdater.WallMeshData,
					MeshUpdateData = _wallMeshUpdateData
				};

				return meshWriteJob.Schedule(wallNormalCalculationJobHandle);
			}

			public void Complete()
			{
				_jobHandle.Complete();
			}

			public void Dispose()
			{
				_meshUpdater.Dispose();
				_surfaceMeshUpdateData.Dispose();
				_wallMeshUpdateData.Dispose();
			}
		}
	}
}
