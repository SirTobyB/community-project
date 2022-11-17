using System;
using System.Collections.Generic;
using System.Diagnostics;
using BoundfoxStudios.CommunityProject.Terrain.Chunks;
using BoundfoxStudios.CommunityProject.Terrain.Jobs;
using BoundfoxStudios.CommunityProject.Terrain.ScriptableObjects;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;
using Grid = BoundfoxStudios.CommunityProject.Terrain.Tiles.Grid;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	[RequireComponent(typeof(Terrain))]
	public class TerrainRenderer : MonoBehaviour
	{
		[SerializeField]
		private Material Material;

		[SerializeField]
		private Terrain Terrain;

		[Header("Listening channels")]
		[SerializeField]
		private UpdateChunksEventChannelSO UpdateChunksEventChannel;

		private readonly List<Chunk> _chunksToUpdate = new();
		private readonly List<Chunk> _chunkCache = new();
		private bool _hasUpdated = false;

		private void LateUpdate()
		{
			UpdateDirtyChunks();
			RenderMeshes();
		}

		private void OnEnable()
		{
			UpdateChunksEventChannel.Raised += MarkChunksDirty;
		}

		private void OnDisable()
		{
			UpdateChunksEventChannel.Raised -= MarkChunksDirty;
		}

		private void MarkChunksDirty(UpdateChunksEventChannelSO.EventArgs args)
		{
			_chunksToUpdate.AddRange(args.Chunks);
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
			if (_hasUpdated)
			{
				return;
			}

			_hasUpdated = true;

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			Debug.Log("Updating ChunkList");
			Profiler.BeginSample("Updating ChunkList");

			var chunkJobPairs = new List<ChunkJobPair>();

			foreach (var chunk in chunksToUpdate)
			{
				var jobPair = new ChunkJobPair()
				{
					Chunk = chunk,
					Grid = Terrain.Grid,
					MaxHeight = Terrain.MaxHeight,
					HeightStep = Terrain.HeightStep
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
			stopwatch.Stop();

			Debug.Log($"Time to construct all chunks: {stopwatch.ElapsedMilliseconds} msec");
		}

		private void RenderMeshes()
		{
			foreach (var chunk in _chunkCache)
			{
				var matrix = transform.localToWorldMatrix;
				chunk.Render(matrix, Material, gameObject.layer);
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

			public void Schedule()
			{
				_meshUpdater = Chunk.AcquireMeshUpdater();
				_surfaceMeshUpdateData = new(Allocator.Persistent);
				_wallMeshUpdateData = new(Allocator.Persistent);

				var surfaceUpdateJobHandle = ScheduleSurfaceUpdate();
				var wallUpdateJobHandle = ScheduleWallUpdate();

				_jobHandle = JobHandle.CombineDependencies(
					surfaceUpdateJobHandle,
					wallUpdateJobHandle
				);
			}

			private JobHandle ScheduleSurfaceUpdate()
			{
				var surfaceChunkJob = new TerrainSurfaceChunkJob()
				{
					MeshUpdateData = _surfaceMeshUpdateData,
					Grid = Grid,
					Bounds = Chunk.Bounds,
					Position = Chunk.Position,
					HeightStep = HeightStep
				};
				var surfaceChunkJobHandle = surfaceChunkJob.Schedule();

				var surfaceNormalCalculationJob = new NormalCalculationJob()
				{
					MeshUpdateData = _surfaceMeshUpdateData
				};
				var surfaceNormalCalculationJobHandle = surfaceNormalCalculationJob.Schedule(surfaceChunkJobHandle);

				var combineNormalsJob = new CombineNormalsJob()
				{
					SurfaceMeshUpdateData = _surfaceMeshUpdateData
				};
				var combineNormalsJobHandle = combineNormalsJob.Schedule(surfaceNormalCalculationJobHandle);

				var meshWriteJob = new WriteChunkMeshJob()
				{
					Bounds = Chunk.GetSubMeshBounds(MaxHeight),
					MeshData = _meshUpdater.SurfaceMeshData,
					MeshUpdateData = _surfaceMeshUpdateData
				};

				return meshWriteJob.Schedule(combineNormalsJobHandle);
			}

			private JobHandle ScheduleWallUpdate()
			{
				var wallChunkJob = new TerrainWallChunkJob()
				{
					MeshUpdateData = _wallMeshUpdateData,
					Grid = Grid,
					Bounds = Chunk.Bounds,
					Position = Chunk.Position,
					HeightStep = HeightStep
				};
				var wallChunkJobHandle = wallChunkJob.Schedule();

				var wallNormalCalculationJob = new NormalCalculationJob()
				{
					MeshUpdateData = _wallMeshUpdateData
				};
				var wallNormalCalculationJobHandle = wallNormalCalculationJob.Schedule(wallChunkJobHandle);

				var meshWriteJob = new WriteChunkMeshJob()
				{
					Bounds = Chunk.GetSubMeshBounds(MaxHeight),
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
