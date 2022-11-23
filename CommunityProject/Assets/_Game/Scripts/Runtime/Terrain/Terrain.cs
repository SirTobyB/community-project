using System;
using System.Collections.Generic;
using BoundfoxStudios.CommunityProject.Infrastructure.SaveManagement;
using BoundfoxStudios.CommunityProject.Terrain.Chunks;
using BoundfoxStudios.CommunityProject.Terrain.ScriptableObjects;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Collections;
using UnityEngine;
using Grid = BoundfoxStudios.CommunityProject.Terrain.Tiles.Grid;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	[AddComponentMenu(Constants.MenuNames.Terrain + "/" + nameof(Terrain))]
	public class Terrain : MonoBehaviour, IHaveSaveData<Terrain.DataContainer>
	{
		[Header("Settings")]
		[SerializeField]
		[Min(32)]
		private int Width = 64;

		[SerializeField]
		[Min(32)]
		private int Length = 64;

		[SerializeField]
		[Min(16)]
		private int ChunkSize = 16;

		internal ChunkList ChunkList;

		[field: SerializeField]
		public TileTypesSO TileTypes { get; private set; }

		[field: SerializeField]
		[field: Range(5, 15)]
		public byte MaxHeight { get; private set; }

		[field: SerializeField]
		[field: Min(0.5f)]
		public float HeightStep { get; private set; } = 1;

		public TerrainDataSO TerrainData;

		public Grid Grid { get; private set; }

		private void Awake()
		{
			Grid = new(Width, Length, MaxHeight, Allocator.Persistent);
			ChunkList = new(new(Width, Length), ChunkSize, MaxHeight);
		}

		private void Start()
		{
			UpdateChunks(ChunkList.Chunks);
		}

		private void OnDestroy()
		{
			Grid.Dispose();
		}

		private void OnValidate()
		{
			if (ChunkSize > Width)
			{
				Debug.LogWarning($"{nameof(ChunkSize)} can't be greater than {nameof(Width)}");
			}

			if (ChunkSize > Length)
			{
				Debug.LogWarning($"{nameof(ChunkSize)} can't be greater than {nameof(Length)}");
			}

			if (Width % ChunkSize != 0)
			{
				Debug.LogWarning($"{nameof(ChunkSize)} must be a multiply of {nameof(Width)}");
			}

			if (Length % ChunkSize != 0)
			{
				Debug.LogWarning($"{nameof(ChunkSize)} must be a multiply of {nameof(Length)}");
			}
		}

		public event Action<IReadOnlyCollection<Chunk>> UpdateChunks = delegate { };

		public class DataContainer
		{
			[JsonProperty("g")]
			public Grid.DataContainer Grid;
		}

		public async UniTask<DataContainer> GetDataContainerAsync() => new()
		{
			Grid = await Grid.GetDataContainerAsync()
		};

		public async UniTask SetDataContainerAsync(DataContainer container)
		{
			var gridContainer = container.Grid;

			Grid.Dispose();
			Grid = new(gridContainer.Width, gridContainer.Length, gridContainer.MaxHeight, Allocator.Persistent);
			await Grid.SetDataContainerAsync(gridContainer);

			ChunkList = new(new(Width, Length), ChunkSize, MaxHeight);
			UpdateChunks(ChunkList.Chunks);
		}
	}
}
