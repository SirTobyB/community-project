using BoundfoxStudios.CommunityProject.Terrain.ScriptableObjects;
using BoundfoxStudios.CommunityProject.Terrain.Tiles;
using Unity.Mathematics;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.Chunks
{
	public class Chunk
	{
		internal Mesh SurfaceMesh { get; } = new();

		internal Mesh WallMesh { get; } = new();

		public IntBounds Bounds { get; }
		public Bounds SubMeshBounds { get; }
		public int2 Position { get; }

		public Chunk(int2 position, IntBounds bounds, byte maxHeight)
		{
			Position = position;
			Bounds = bounds;
			SubMeshBounds = GetSubMeshBounds(maxHeight);
		}

		public ChunkMeshUpdater AcquireMeshUpdater()
		{
			return new(SurfaceMesh, WallMesh, SubMeshBounds);
		}

		private Bounds GetSubMeshBounds(byte maxHeight) =>
			new(
				new(Position.x + Bounds.Center.x, (float) maxHeight / 2, Position.y + Bounds.Center.y),
				new(Bounds.Size.x, maxHeight, Bounds.Size.y)
			);

		public void Render(Matrix4x4 matrix, TileTypesSO tileTypes, int layer)
		{
			for (var i = 0; i < SurfaceMesh.subMeshCount; i++)
			{
				Graphics.DrawMesh(SurfaceMesh, matrix, tileTypes.ById((byte)i).SurfaceMaterial, layer, null, i);
			}

			for (var i = 0; i < WallMesh.subMeshCount; i++)
			{
				Graphics.DrawMesh(WallMesh, matrix, tileTypes.ById((byte)i).WallMaterial, layer, null, i);
			}

		}
	}
}
