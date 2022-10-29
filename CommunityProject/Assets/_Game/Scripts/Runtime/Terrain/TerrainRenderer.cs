using System;
using System.Collections.Generic;
using System.Diagnostics;
using BoundfoxStudios.CommunityProject.Terrain.Core;
using BoundfoxStudios.CommunityProject.Terrain.Core.ScriptableObjects;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;
using Grid = BoundfoxStudios.CommunityProject.Terrain.Core.Grid;

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
			Debug.Log("Updating Chunks");
			Profiler.BeginSample("Updating Chunks");

			var chunkJobPairs = new List<ChunkJobPair>();

			foreach (var chunk in chunksToUpdate)
			{
				chunkJobPairs.Add(new(chunk, Terrain.Grid, Terrain.MaxHeight, Terrain.HeightStep));
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
			private readonly ChunkMeshUpdater _meshUpdater;
			private JobHandle _jobHandle;
			private NativeMeshUpdateData _terrainTileMeshUpdateData;
			private NativeMeshUpdateData _terrainWallMeshUpdateData;

			public Chunk Chunk { get; }

			public ChunkJobPair(Chunk chunk, Grid grid, byte maxHeight, float heightStep)
			{
				Chunk = chunk;
				_meshUpdater = chunk.AcquireMeshUpdater();
				_terrainTileMeshUpdateData = new(Allocator.Persistent);
				_terrainWallMeshUpdateData = new(Allocator.Persistent);

				var terrainTileChunkJob = new TerrainTileChunkJob()
				{
					MeshUpdateData = _terrainTileMeshUpdateData,
					Grid = grid,
					Bounds = chunk.Bounds,
					Position = chunk.Position,
					HeightStep = heightStep
				};
				var terrainChunkJobHandle = terrainTileChunkJob.Schedule();

				var terrainWallChunkJob = new TerrainWallChunkJob()
				{
					MeshUpdateData = _terrainWallMeshUpdateData,
					Grid = grid,
					Bounds = chunk.Bounds,
					Position = chunk.Position,
					HeightStep = heightStep
				};
				var terrainWallChunkJobHandle = terrainWallChunkJob.Schedule();

				var surfaceMeshWriteJob = new WriteChunkMeshJob()
				{
					Bounds = chunk.GetSubMeshBounds(maxHeight),
					MeshData = _meshUpdater.SurfaceMeshData,
					MeshUpdateData = _terrainTileMeshUpdateData
				};

				var wallMeshWriteJob = new WriteChunkMeshJob()
				{
					Bounds = chunk.GetSubMeshBounds(maxHeight),
					MeshData = _meshUpdater.WallMeshData,
					MeshUpdateData = _terrainWallMeshUpdateData
				};

				_jobHandle = JobHandle.CombineDependencies(
					surfaceMeshWriteJob.Schedule(terrainChunkJobHandle),
					wallMeshWriteJob.Schedule(terrainWallChunkJobHandle)
				);
			}

			public void Complete()
			{
				_jobHandle.Complete();
			}

			public void Dispose()
			{
				_meshUpdater.Dispose();
				_terrainTileMeshUpdateData.Dispose();
				_terrainWallMeshUpdateData.Dispose();
			}
		}
	}
}
