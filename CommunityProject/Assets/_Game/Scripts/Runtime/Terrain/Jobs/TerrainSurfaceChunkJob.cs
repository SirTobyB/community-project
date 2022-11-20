using BoundfoxStudios.CommunityProject.Terrain.Tiles;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace BoundfoxStudios.CommunityProject.Terrain.Jobs
{
	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	public struct TerrainSurfaceChunkJob : IJob
	{
		public NativeMeshUpdateData MeshUpdateData;

		[ReadOnly]
		public Grid Grid;

		[ReadOnly]
		public IntBounds Bounds;

		[ReadOnly]
		public float HeightStep;

		private float3 GetCornerPosition(ref Tile tile, Corner corner)
			=> tile.GetCornerPosition(corner) * new float3(1, HeightStep, 1);

		public void Execute()
		{
			for (var x = Bounds.Min.x; x < Bounds.Max.x; x++)
			{
				for (var z = Bounds.Min.y; z < Bounds.Max.y; z++)
				{
					var position = new int2(x, z);

					var tile = Grid.GetTile(position);

					GenerateTile(ref tile);
				}
			}
		}

		private void GenerateTile(ref Tile tile)
		{
			var bottomCenter = tile.BottomCenter;

			var cornerNorthWest = GetCornerPosition(ref tile, Corner.NorthWest);
			var cornerNorthEast = GetCornerPosition(ref tile, Corner.NorthEast);
			var cornerSouthEast = GetCornerPosition(ref tile, Corner.SouthEast);
			var cornerSouthWest = GetCornerPosition(ref tile, Corner.SouthWest);
			var center = bottomCenter + new float3(0, tile.Center * HeightStep, 0);

			var vertexIndex = MeshUpdateData.Vertices.Length;
			var centerVertexIndex = vertexIndex;
			var northWestVertexIndex = vertexIndex + 1;
			var northEastVertexIndex = vertexIndex + 2;
			var southEastVertexIndex = vertexIndex + 3;
			var southWestVertexIndex = vertexIndex + 4;

			var vertex = new Vertex();

			// Vertex: Center
			vertex.Position = center;
			vertex.TexCoord0 = new(0.5f, 0.5f);
			MeshUpdateData.Vertices.Add(vertex);

			// Vertex: North West
			vertex.Position = cornerNorthWest;
			vertex.TexCoord0 = new(0, 1);
			MeshUpdateData.Vertices.Add(vertex);

			// Vertex: North East
			vertex.Position = cornerNorthEast;
			vertex.TexCoord0 = new(1, 1);
			MeshUpdateData.Vertices.Add(vertex);

			// Vertex: South East
			vertex.Position = cornerSouthEast;
			vertex.TexCoord0 = new(1, 0);
			MeshUpdateData.Vertices.Add(vertex);

			// Vertex: South West
			vertex.Position = cornerSouthWest;
			vertex.TexCoord0 = new(0, 0);
			MeshUpdateData.Vertices.Add(vertex);

			var northTileType = tile.GetTileType(Direction.North);
			var eastTileType = tile.GetTileType(Direction.East);
			var southTileType = tile.GetTileType(Direction.South);
			var westTileType = tile.GetTileType(Direction.West);

			MeshUpdateData.Triangles.Add(new (northTileType, centerVertexIndex, northWestVertexIndex, northEastVertexIndex));
			MeshUpdateData.Triangles.Add(new (eastTileType, centerVertexIndex, northEastVertexIndex, southEastVertexIndex));
			MeshUpdateData.Triangles.Add(new (southTileType, centerVertexIndex, southEastVertexIndex, southWestVertexIndex));
			MeshUpdateData.Triangles.Add(new (westTileType, centerVertexIndex, southWestVertexIndex, northWestVertexIndex));
		}
	}
}
