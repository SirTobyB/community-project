using System.Collections.Generic;
using BoundfoxStudios.CommunityProject.Events.ScriptableObjects;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.Core.ScriptableObjects
{
	[CreateAssetMenu(menuName = Constants.MenuNames.Events + "/Update Chunks")]
	public class UpdateChunksEventChannelSO : EventChannelSO<UpdateChunksEventChannelSO.EventArgs>
	{
		public struct EventArgs
		{
			public IReadOnlyCollection<Chunk> Chunks;
		}
	}
}
