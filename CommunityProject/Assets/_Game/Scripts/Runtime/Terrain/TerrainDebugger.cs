using System;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	[AddComponentMenu(Constants.MenuNames.Terrain + "/" + nameof(TerrainDebugger))]
	public class TerrainDebugger : MonoBehaviour
	{
		[Serializable]
		public class DebuggerOptions
		{
			public bool ShowChunkBoundaries = true;

			[Tooltip("Careful, showing normals on large terrains has a serious performance impact!")]
			public bool ShowNormals;
		}

		public Terrain Terrain;
		public DebuggerOptions Options;
	}
}
