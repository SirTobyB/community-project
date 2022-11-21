using System;
using BoundfoxStudios.CommunityProject.Terrain.Jobs.Calculations;
using BoundfoxStudios.CommunityProject.Terrain.Tiles;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Android;

namespace BoundfoxStudios.CommunityProject.Terrain.Jobs
{
	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	public struct SingleTriangleJob : IJob
	{
		public NativeMeshUpdateData MeshUpdateData;

		[ReadOnly]
		public Grid Grid;

		[ReadOnly]
		public int2 Position;

		[ReadOnly]
		public float HeightStep;

		[ReadOnly]
		public Direction TriangleDirection;

		public void Execute()
		{
			var tile = Grid.GetTile(Position);

			var firstCorner = TriangleDirection.ABC switch
			{
				Directions.North => Corner.NorthWest,
				Directions.East => Corner.NorthEast,
				Directions.South => Corner.SouthEast,
				Directions.West => Corner.SouthWest,
				_ => throw new ArgumentOutOfRangeException(nameof(TriangleDirection))
			};

			var secondCorner = firstCorner.NeighborClockwise;

			var centerPosition = SurfaceCalculations.GetCenterPosition(ref tile, HeightStep);
			var firstCornerPosition = SurfaceCalculations.GetCornerPosition(ref tile, firstCorner, HeightStep);
			var secondCornerPosition = SurfaceCalculations.GetCornerPosition(ref tile, secondCorner, HeightStep);

			var vertex = new Vertex();

			// Vertex: Center
			vertex.Position = centerPosition;
			vertex.TexCoord0 = new(0.5f, 0.5f);
			MeshUpdateData.Vertices.Add(vertex);

			// Vertex: First Corner
			vertex.Position = firstCornerPosition;
			vertex.TexCoord0 = firstCorner.TexCoord0;
			MeshUpdateData.Vertices.Add(vertex);

			// Vertex: Second Corner
			vertex.Position = secondCornerPosition;
			vertex.TexCoord0 = secondCorner.TexCoord0;
			MeshUpdateData.Vertices.Add(vertex);

			var tileType = tile.GetTileType(TriangleDirection);

			MeshUpdateData.Triangles.Add(new(tileType, 0, 1, 2));
		}
	}
}
