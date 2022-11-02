using Unity.Mathematics;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.Core
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

		public void Render(Matrix4x4 matrix, Material material, int layer)
		{
			Graphics.DrawMesh(_surfaceMesh, matrix, material, layer);
			Graphics.DrawMesh(_wallMesh, matrix, material, layer);
		}
	}
}
