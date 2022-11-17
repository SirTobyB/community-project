using System.Collections.Generic;
using BoundfoxStudios.CommunityProject.Events.ScriptableObjects;
using BoundfoxStudios.CommunityProject.Terrain.Chunks;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.ScriptableObjects
{
	[CreateAssetMenu(menuName = Constants.MenuNames.Events + "/Update ChunkList")]
	public class UpdateChunksEventChannelSO : EventChannelSO<UpdateChunksEventChannelSO.EventArgs>
	{
		public struct EventArgs
		{
			public IReadOnlyCollection<Chunk> Chunks;
		}
	}
}
