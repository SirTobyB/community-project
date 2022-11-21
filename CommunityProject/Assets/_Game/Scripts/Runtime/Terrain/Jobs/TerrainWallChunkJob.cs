using BoundfoxStudios.CommunityProject.Terrain.Tiles;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace BoundfoxStudios.CommunityProject.Terrain.Jobs
{
	/// <summary>
	///   This job generates walls for a chunk.
	/// </summary>
	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	public struct TerrainWallChunkJob : IJob
	{
		[ReadOnly]
		public Grid Grid;

		[ReadOnly]
		public IntBounds Bounds;

		[ReadOnly]
		public float HeightStep;

		public NativeMeshUpdateData MeshUpdateData;

		private float3 _heightStep;

		public void Execute()
		{
			_heightStep = new(1, HeightStep, 1);

			for (var x = Bounds.Min.x; x < Bounds.Max.x; x++)
			{
				for (var z = Bounds.Min.y; z < Bounds.Max.y; z++)
				{
					var position = new int2(x, z);

					var tile = Grid.GetTile(position);

					GenerateWallsFor(ref tile);
				}
			}
		}

		private void GenerateWallsFor(ref Tile tile)
		{
			GenerateWalls(ref tile, Direction.North, Corner.NorthWest);
			GenerateWalls(ref tile, Direction.East, Corner.NorthEast);
			GenerateWalls(ref tile, Direction.South, Corner.SouthEast);
			GenerateWalls(ref tile, Direction.West, Corner.SouthWest);
		}

		private void GenerateWalls(ref Tile tile, Direction direction, Corner corner)
		{
			var neighbor = tile.GetNeighbor(direction);
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

			var tileType = tile.GetTileType(direction);

			//TODO: Extract another Method for this
			if (needsTriangle1)
			{
				var vertexIndex = MeshUpdateData.Vertices.Length;

				AddNewVertexToVertices(ref tile, corner.NeighborClockwise, new(0, 1));
				AddNewVertexToVertices(ref tile, corner, new(1, 1));
				AddNewVertexToVertices(ref neighbor, neighborLeft, new(1, 0));
				MeshUpdateData.Triangles.Add(new(tileType, vertexIndex, vertexIndex + 1, vertexIndex + 2));
			}

			if (needsTriangle2)
			{
				var vertexIndex = MeshUpdateData.Vertices.Length;

				AddNewVertexToVertices(ref tile, corner.NeighborClockwise, new(0, 1));
				if (needsTriangle1)
				{
					AddNewVertexToVertices(ref neighbor, neighborLeft, new(1, 0));
				}
				else
				{
					AddNewVertexToVertices(ref tile, corner, new(1, 1));
				}

				AddNewVertexToVertices(ref neighbor, neighborRight, new(0, 0));
				MeshUpdateData.Triangles.Add(new(tileType, vertexIndex, vertexIndex + 1, vertexIndex + 2));
			}
		}

		private readonly void AddNewVertexToVertices(ref Tile tile, Corner corner, float2 texCoord)
		{
			var vertex = new Vertex
			{
				Position = _heightStep * tile.GetCornerPosition(corner),
				TexCoord0 = texCoord
			};
			MeshUpdateData.Vertices.Add(vertex);
		}
	}
}
