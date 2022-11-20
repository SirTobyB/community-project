using System;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.Diagnostics
{
	[AddComponentMenu(Constants.MenuNames.Terrain + "/" + nameof(TerrainDebugger))]
	public class TerrainDebugger : MonoBehaviour
	{
		public Terrain Terrain;
		public DebuggerOptions Options;

		[Serializable]
		public class DebuggerOptions
		{
			public bool ShowChunkBoundaries = true;

			[Tooltip("Careful, showing normals on large terrains has a serious performance impact!")]
			public bool ShowNormals;
		}
	}
}
