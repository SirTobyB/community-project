using BoundfoxStudios.CommunityProject.Terrain.Core;
using BoundfoxStudios.CommunityProject.Terrain.Core.ScriptableObjects;
using Unity.Collections;
using UnityEngine;
using Grid = BoundfoxStudios.CommunityProject.Terrain.Core.Grid;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	public class Terrain : MonoBehaviour
	{
		[Header("Settings")]
		[SerializeField]
		[Min(32)]
		private int Width = 64;

		[SerializeField]
		[Min(32)]
		private int Length = 64;

		[field: SerializeField]
		[field: Range(5, 15)]
		public byte MaxHeight { get; private set; }

		[SerializeField]
		[Min(16)]
		private int ChunkSize = 16;

		[field:SerializeField]
		[field:Min(0.5f)]
		public float HeightStep { get; private set; } = 1;

		[Header("Broadcasting channels")]
		[SerializeField]
		private UpdateChunksEventChannelSO UpdateChunksEventChannel;

		private Grid _grid;
		public Grid Grid => _grid;
		internal Chunks Chunks;

		private void Awake()
		{
			_grid = new(Width, Length, MaxHeight, Allocator.Persistent);
			Chunks = new(new(Width, Length), ChunkSize);
		}

		private void Start()
		{
			// TODO: For Testing
			UpdateChunksEventChannel.Raise(new()
			{
				Chunks = Chunks
			});
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

		private void OnDestroy()
		{
			_grid.Dispose();
		}
	}
}
