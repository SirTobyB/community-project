using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.Core
{
	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	public struct TerrainWallChunkJob : IJob
	{
		[ReadOnly]
		public Grid Grid;

		[ReadOnly]
		public int2 Position;

		[ReadOnly]
		public IntBounds Bounds;

		[ReadOnly]
		public float HeightStep;

		public NativeMeshUpdateData MeshUpdateData;

		private float3 _heightStep;

		public void Execute()
		{
			_heightStep = new(1, HeightStep, 1);

			for (var x = 0; x < Bounds.Size.x; x++)
			{
				for (var z = 0; z < Bounds.Size.y; z++)
				{
					var position = new int2(Position.x + x, Position.y + z);

					var tile = Grid.GetTile(position);

					GenerateWallsFor(ref tile);
				}
			}
		}

		private void GenerateWallsFor(ref Tile tile)
		{
			GenerateWalls(ref tile, tile.GetNeighbor(Direction.North), Corner.NorthWest);
			GenerateWalls(ref tile, tile.GetNeighbor(Direction.East), Corner.NorthEast);
			GenerateWalls(ref tile, tile.GetNeighbor(Direction.South), Corner.SouthEast);
			GenerateWalls(ref tile, tile.GetNeighbor(Direction.West), Corner.SouthWest);
		}

		private void GenerateWalls(ref Tile tile, Tile neighbor, Corner corner)
		{
			var neighborLeft = corner.NeighborCounterClockwise;
			var neighborRight = corner.NeighborOpposite;

			var tileData = tile.GetData();

			var isNeighborInBounds = neighbor.IsInBounds;
			var needsTriangle1 = !isNeighborInBounds || neighbor.GetHeight(neighborLeft) < tileData.GetHeight(corner);
			var needsTriangle2 = !isNeighborInBounds
			                     || neighbor.GetHeight(neighborRight) < tileData.GetHeight(corner.NeighborClockwise);

			if (!needsTriangle1 && !needsTriangle2)
			{
				return;
			}

			var vertexIndex = MeshUpdateData.Vertices.Length;

			var vertex = new Vertex();

			vertex.Position = _heightStep * tile.GetCornerPosition(corner.NeighborClockwise);
			vertex.TexCoord0 = new(0, 1);
			MeshUpdateData.Vertices.Add(vertex);

			vertex.Position = _heightStep * tile.GetCornerPosition(corner);
			vertex.TexCoord0 = new(1, 1);
			MeshUpdateData.Vertices.Add(vertex);

			if (needsTriangle1 && needsTriangle2)
			{
				vertex.Position = _heightStep * neighbor.GetCornerPosition(neighborLeft);
				vertex.TexCoord0 = new(1, 0);
				MeshUpdateData.Vertices.Add(vertex);

				vertex.Position = _heightStep * neighbor.GetCornerPosition(neighborRight);
				vertex.TexCoord0 = new(0, 0);
				MeshUpdateData.Vertices.Add(vertex);

				MeshUpdateData.Triangles.Add(new(vertexIndex, vertexIndex + 1, vertexIndex + 3));
				MeshUpdateData.Triangles.Add(new(vertexIndex + 1, vertexIndex + 2, vertexIndex + 3));
				return;
			}

			if (needsTriangle1)
			{
				vertex.Position = _heightStep * neighbor.GetCornerPosition(neighborLeft);
				vertex.TexCoord0 = new(1, 0);
				MeshUpdateData.Vertices.Add(vertex);
			}
			else
			{
				vertex.Position = _heightStep * neighbor.GetCornerPosition(neighborRight);
				vertex.TexCoord0 = new(0, 0);
				MeshUpdateData.Vertices.Add(vertex);
			}

			MeshUpdateData.Triangles.Add(new(vertexIndex, vertexIndex + 1, vertexIndex + 2));
		}
	}
}
