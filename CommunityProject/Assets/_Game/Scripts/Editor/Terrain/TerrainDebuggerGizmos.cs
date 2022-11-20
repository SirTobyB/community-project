using BoundfoxStudios.CommunityProject.Terrain;
using BoundfoxStudios.CommunityProject.Terrain.Chunks;
using BoundfoxStudios.CommunityProject.Terrain.Diagnostics;
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
				DrawChunk(chunk, terrainDebugger.transform.position, terrainDebugger.Terrain.MaxHeight,
					terrainDebugger.Options);
			}
		}

		private static void DrawChunk(Chunk chunk, Vector3 terrainPosition, byte maxHeight,
			TerrainDebugger.DebuggerOptions options)
		{
			Gizmos.color = Color.yellow;

			if (options.ShowChunkBoundaries)
			{
				DrawChunkBoundary(chunk, terrainPosition, maxHeight);
			}

			if (options.ShowNormals)
			{
				DrawChunkNormals(chunk);
			}
		}

		private static void DrawChunkBoundary(Chunk chunk, Vector3 terrainPosition, byte maxHeight)
		{
			var center = terrainPosition + new Vector3(chunk.Bounds.Center.x, maxHeight / 2f,
				chunk.Bounds.Center.y);
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
