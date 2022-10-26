using System;
using System.Collections.Generic;
using System.Diagnostics;
using BoundfoxStudios.CommunityProject.Terrain.Core;
using BoundfoxStudios.CommunityProject.Terrain.Core.ScriptableObjects;
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
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			Debug.Log("Updating Chunks");
			Profiler.BeginSample("Updating Chunks");

			var chunkJobPairs = new List<ChunkJobPair>();

			foreach (var chunk in chunksToUpdate)
			{
				 chunkJobPairs.Add(new(chunk, Terrain.Grid));
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
			private readonly TerrainChunkMeshUpdater _meshUpdater;
			private JobHandle _jobHandle;
			private  TerrainChunkJob _terrainChunkJob;

			public Chunk Chunk { get; }

			public ChunkJobPair(Chunk chunk, Grid grid)
			{
				Chunk = chunk;
				_meshUpdater = chunk.AcquireMeshUpdater();

				_terrainChunkJob = new(_meshUpdater.MeshData, grid, chunk.Position, chunk.Bounds);
				_jobHandle = _terrainChunkJob.Schedule();
			}

			public void Complete()
			{
				_jobHandle.Complete();
			}

			public void Dispose()
			{
				_meshUpdater.Dispose();
				_terrainChunkJob.Dispose();
			}
		}
	}
}
