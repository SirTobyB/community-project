using BoundfoxStudios.CommunityProject.Terrain.ScriptableObjects;
using BoundfoxStudios.CommunityProject.Terrain.Tiles;
using Unity.Mathematics;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.Chunks
{
	public class Chunk
	{
		private readonly Mesh _surfaceMesh = new();
		private readonly Mesh _wallMesh = new();

		internal Mesh SurfaceMesh => _surfaceMesh;
		internal Mesh WallMesh => _wallMesh;

		public IntBounds Bounds { get; }
		public int2 Position { get; }

		public Chunk(int2 position, IntBounds bounds)
		{
			Position = position;
			Bounds = bounds;
		}

		public ChunkMeshUpdater AcquireMeshUpdater()
		{
			return new(_surfaceMesh, _wallMesh);
		}

		public Bounds GetSubMeshBounds(byte maxHeight) =>
			new(
				new(Position.x + Bounds.Center.x, (float) maxHeight / 2, Position.y + Bounds.Center.y),
				new(Bounds.Size.x, maxHeight, Bounds.Size.y)
			);

		public void Render(Matrix4x4 matrix, TileTypesSO tileTypes, int layer)
		{
			for (var i = 0; i < _surfaceMesh.subMeshCount; i++)
			{
				Graphics.DrawMesh(_surfaceMesh, matrix, tileTypes.ById((byte)i).SurfaceMaterial, layer, null, i);
			}

			for (var i = 0; i < _wallMesh.subMeshCount; i++)
			{
				Graphics.DrawMesh(_wallMesh, matrix, tileTypes.ById((byte)i).WallMaterial, layer, null, i);
			}

		}
	}
}
