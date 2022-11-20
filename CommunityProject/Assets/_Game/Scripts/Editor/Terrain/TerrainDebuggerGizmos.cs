using BoundfoxStudios.CommunityProject.Terrain;
using BoundfoxStudios.CommunityProject.Terrain.Chunks;
using UnityEditor;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Editor.Terrain
{
	public static class TerrainGizmos
	{
		[DrawGizmo(GizmoType.Selected, typeof(TerrainDebugger))]
		private static void DrawChunksGizmos(TerrainDebugger terrainDebugger, GizmoType gizmoType)
		{
			if (!EditorApplication.isPlaying)
			{
				return;
			}

			foreach (var chunk in terrainDebugger.Terrain.ChunkList)
			{
				DrawChunk(chunk, terrainDebugger.Terrain.MaxHeight, terrainDebugger.Options);
			}
		}

		private static void DrawChunk(Chunk chunk, byte maxHeight, TerrainDebugger.DebuggerOptions options)
		{
			Gizmos.color = Color.yellow;

			if (options.ShowChunkBoundaries)
			{
				DrawChunkBoundary(chunk, maxHeight);
			}

			if (options.ShowNormals)
			{
				DrawChunkNormals(chunk);
			}
		}

		private static void DrawChunkBoundary(Chunk chunk, byte maxHeight)
		{
			var center = new Vector3(chunk.Position.x + chunk.Bounds.Size.x / 2, maxHeight / 2f,
				chunk.Position.y + chunk.Bounds.Size.y / 2);
			var size = new Vector3(chunk.Bounds.Size.x, maxHeight, chunk.Bounds.Size.y);

			Gizmos.DrawWireCube(center, size);
		}

		private static void DrawChunkNormals(Chunk chunk)
		{
			DrawNormals(chunk.SurfaceMesh);
			DrawNormals(chunk.WallMesh);
		}

		private static void DrawNormals(Mesh mesh)
		{
			var normals = mesh.normals;
			var vertices = mesh.vertices;

			for (var index = 0; index < normals.Length; index++)
			{
				var normal = normals[index];
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(vertices[index], 0.05f);

				Gizmos.color = Color.red;
				Gizmos.DrawLine(vertices[index], vertices[index] + normal * 0.5f);
			}
		}
	}
}
