using BoundfoxStudios.CommunityProject.Terrain.Core;
using UnityEditor;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Editor.Terrain
{
	public static class TerrainGizmos
	{
		[DrawGizmo(GizmoType.Selected, typeof(CommunityProject.Terrain.Terrain))]
		private static void DrawChunksGizmos(CommunityProject.Terrain.Terrain terrain, GizmoType gizmoType)
		{
			if (!EditorApplication.isPlaying)
			{
				return;
			}

			foreach (var chunk in terrain.Chunks)
			{
				DrawChunk(chunk);
			}
		}

		private static void DrawChunk(Chunk chunk)
		{
			Gizmos.color = Color.yellow;

			// TODO: Height of a chunk?
			var center = new Vector3(chunk.Position.x + chunk.Bounds.Size.x / 2, 5, chunk.Position.y + chunk.Bounds.Size.y / 2);
			var size = new Vector3(chunk.Bounds.Size.x, 10, chunk.Bounds.Size.y);

			Gizmos.DrawWireCube(center, size);
		}
	}
}
