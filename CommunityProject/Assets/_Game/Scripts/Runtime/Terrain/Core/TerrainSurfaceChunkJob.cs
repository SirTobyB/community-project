using System;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace BoundfoxStudios.CommunityProject.Terrain.Core
{
	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	public struct TerrainChunkJob : IJob, IDisposable
	{
		[ReadOnly]
		private readonly Grid _grid;

		[ReadOnly]
		private readonly int2 _position;

		[NativeDisableContainerSafetyRestriction]
		private NativeArray<StreamVertex> _stream;

		[NativeDisableContainerSafetyRestriction]
		private NativeArray<ushort> _triangles;

		[ReadOnly]
		private readonly IntBounds _bounds;

		private readonly float _heightStep;

		[ReadOnly]
		private NativeArray<float3> _offsets;

		[StructLayout(LayoutKind.Sequential)]
		private struct StreamVertex
		{
			public float3 Position;
			public float3 Normal;
			public float2 TexCoord0;
		}

		public TerrainChunkJob(
			Mesh.MeshData meshData,
			Grid grid,
			int2 position,
			IntBounds bounds,
			float heightStep
		)
		{
			_grid = grid;
			_position = position;
			_bounds = bounds;
			_heightStep = heightStep;

			var descriptor =
				new NativeArray<VertexAttributeDescriptor>(3, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

			descriptor[0] = new(dimension: 3);
			descriptor[1] = new(VertexAttribute.Normal, dimension: 3);
			descriptor[2] = new(VertexAttribute.TexCoord0, dimension: 2);

			var tileCount = _bounds.Size.x * _bounds.Size.y;

			const int verticesPerTile = 5;
			var vertexCount = tileCount * verticesPerTile;
			meshData.SetVertexBufferParams(vertexCount, descriptor);
			descriptor.Dispose();

			const int trianglesPerTile = 4;
			const int verticesPerTriangle = 3;
			var triangleIndexCount = tileCount * trianglesPerTile * verticesPerTriangle;
			meshData.SetIndexBufferParams(triangleIndexCount, IndexFormat.UInt16);

			// TODO: Height?
			var subMeshBounds = new Bounds(
				new(_position.x + bounds.Center.x, 5, _position.y + bounds.Center.y),
				new(bounds.Size.x, 10, bounds.Size.y)
			);

			meshData.subMeshCount = 1;
			meshData.SetSubMesh(0, new(0, triangleIndexCount)
			{
				vertexCount = vertexCount,
				bounds = subMeshBounds
			}, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);

			_stream = meshData.GetVertexData<StreamVertex>();
			_triangles = meshData.GetIndexData<ushort>();

			_offsets = new(4, Allocator.TempJob);
			_offsets[0] = Tile.CornerOffsetsFromCenter[0];
			_offsets[1] = Tile.CornerOffsetsFromCenter[1];
			_offsets[2] = Tile.CornerOffsetsFromCenter[2];
			_offsets[3] = Tile.CornerOffsetsFromCenter[3];
		}

		private float3 GetCornerPosition(float3 bottomCenter, TileData tileData, Corner corner) =>
			_offsets[corner.Index] + bottomCenter + new float3(0, tileData.GetHeight(corner) * _heightStep, 0);

		public void Execute()
		{
			for (var x = 0; x < _bounds.Size.x; x++)
			{
				for (var z = 0; z < _bounds.Size.y; z++)
				{
					var position = new int2(_position.x + x, _position.y + z);

					var tile = _grid.GetTile(position);

					GenerateTile(x + z * _bounds.Size.x, ref tile);
				}
			}
		}

		private void GenerateTile(int index, ref Tile tile)
		{
			var tileData = tile.GetData();
			var bottomCenter = tile.BottomCenter;

			var cornerNorthWest = GetCornerPosition(bottomCenter, tileData, Corner.NorthWest);
			var cornerNorthEast = GetCornerPosition(bottomCenter, tileData, Corner.NorthEast);
			var cornerSouthEast = GetCornerPosition(bottomCenter, tileData, Corner.SouthEast);
			var cornerSouthWest = GetCornerPosition(bottomCenter, tileData, Corner.SouthWest);
			var center = bottomCenter + new float3(0, tile.Center * _heightStep, 0);

			var vertexOffset = index * 5;
			var centerVertexIndex = vertexOffset;
			var northWestVertexIndex = vertexOffset + 1;
			var northEastVertexIndex = vertexOffset + 2;
			var southEastVertexIndex = vertexOffset + 3;
			var southWestVertexIndex = vertexOffset + 4;

			var indexOffset = index * 12;

			var vertex = new StreamVertex()
			{
				Normal = math.up()
			};

			// Vertex: Center
			vertex.Position = center;
			vertex.TexCoord0 = new(0.5f, 0.5f);
			_stream[centerVertexIndex] = vertex;

			// Vertex: North West
			vertex.Position = cornerNorthWest;
			vertex.TexCoord0 = new(0, 1);
			_stream[northWestVertexIndex] = vertex;

			// Vertex: North East
			vertex.Position = cornerNorthEast;
			vertex.TexCoord0 = new(1, 1);
			_stream[northEastVertexIndex] = vertex;

			// Vertex: South East
			vertex.Position = cornerSouthEast;
			vertex.TexCoord0 = new(1, 0);
			_stream[southEastVertexIndex] = vertex;

			// Vertex: South West
			vertex.Position = cornerSouthWest;
			vertex.TexCoord0 = new(0, 0);
			_stream[southWestVertexIndex] = vertex;

			// Triangle: North
			AddTriangle(indexOffset, 0, centerVertexIndex, northWestVertexIndex, northEastVertexIndex);

			// Triangle: East
			AddTriangle(indexOffset, 3, centerVertexIndex, northEastVertexIndex, southEastVertexIndex);

			// Triangle: South
			AddTriangle(indexOffset, 6, centerVertexIndex, southEastVertexIndex, southWestVertexIndex);

			// Triangle: West
			AddTriangle(indexOffset, 9, centerVertexIndex, southWestVertexIndex, northWestVertexIndex);
		}

		private void AddTriangle(int indexOffset, int relativeOffset, int vertex1, int vertex2, int vertex3)
		{
			_triangles[indexOffset + relativeOffset] = (ushort)vertex1;
			_triangles[indexOffset + relativeOffset + 1] = (ushort)vertex2;
			_triangles[indexOffset + relativeOffset + 2] = (ushort)vertex3;
		}

		public void Dispose()
		{
			_stream.Dispose();
			_triangles.Dispose();
			_offsets.Dispose();
		}
	}
}
