using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace BoundfoxStudios.CommunityProject.Terrain.Jobs
{
	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	public struct WriteChunkMeshJob : IJob
	{
		public Mesh.MeshData MeshData;

		[ReadOnly]
		public NativeMeshUpdateData MeshUpdateData;

		[ReadOnly]
		public Bounds Bounds;

		private const int VerticesPerTriangle = 3;

		[StructLayout(LayoutKind.Sequential)]
		private struct StreamVertex
		{
			public float3 Position;
			public float3 Normal;
			public float2 TexCoord0;
		}

		public void Execute()
		{
			Initialize();
			WriteVertices();
			WriteSubMeshes();
		}

		private void Initialize()
		{
			var descriptor =
				new NativeArray<VertexAttributeDescriptor>(3, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

			descriptor[0] = new(dimension: 3);
			descriptor[1] = new(VertexAttribute.Normal, dimension: 3);
			descriptor[2] = new(VertexAttribute.TexCoord0, dimension: 2);

			var vertexCount = MeshUpdateData.Vertices.Length;
			MeshData.SetVertexBufferParams(vertexCount, descriptor);
			descriptor.Dispose();

			var triangleIndexCount = VerticesPerTriangle * MeshUpdateData.Triangles.Length;
			MeshData.SetIndexBufferParams(triangleIndexCount, IndexFormat.UInt16);
		}

		private void WriteSubMeshes()
		{
			var subMeshCount = MeshUpdateData.SubMeshCount;
			MeshData.subMeshCount = subMeshCount;

			var indexBufferStream = MeshData.GetIndexData<ushort>();

			var previousStartIndex = 0;
			for (var i = 0; i < subMeshCount; i++)
			{
				var triangles = GetTileTypeTriangles((byte)i);

				var subMeshTriangleStartIndex = previousStartIndex;
				var subMeshTriangleIndexCount = VerticesPerTriangle * triangles.Length;

				var (firstVertex, vertexCount) = GetFirstIndexAndVertexCount(ref triangles);

				MeshData.SetSubMesh(i, new(subMeshTriangleStartIndex, subMeshTriangleIndexCount)
				{
					firstVertex = firstVertex,
					vertexCount = vertexCount,
					bounds = Bounds
				}, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);

				WriteTriangles(ref triangles, ref indexBufferStream, previousStartIndex);

				triangles.Dispose();

				previousStartIndex += subMeshTriangleIndexCount;
			}
		}

		private (int FirstVertex, int VertexCount) GetFirstIndexAndVertexCount(ref NativeList<Triangle> triangles)
		{
			if (triangles.IsEmpty)
			{
				return (0, 0);
			}

			var (minimumVertexIndex, maximumVertexIndex) = GetMinimumAndMaximumVertexIndex(ref triangles);

			return (
				minimumVertexIndex,
				maximumVertexIndex - minimumVertexIndex + 1
			);
		}

		private (int MinimumVertexIndex, int MaximumVertexIndex) GetMinimumAndMaximumVertexIndex(
			ref NativeList<Triangle> triangles)
		{
			var minimum = int.MaxValue;
			var maximum = int.MinValue;

			for (var index = 0; index < triangles.Length; index++)
			{
				var triangle = triangles[index];

				var (triangleMinimum, triangleMaximum) = GetMinimumAndMaximumTriangleVertexIndex(ref triangle);

				minimum = math.min(minimum, triangleMinimum);
				maximum = math.max(maximum, triangleMaximum);
			}

			return (minimum, maximum);
		}

		private (int MinimumVertexIndex, int MaximumVertexIndex) GetMinimumAndMaximumTriangleVertexIndex(
			ref Triangle triangle)
		{
			var minimum = int.MaxValue;
			var maximum = int.MinValue;

			minimum = math.min(minimum, triangle.VertexIndex1);
			minimum = math.min(minimum, triangle.VertexIndex2);
			minimum = math.min(minimum, triangle.VertexIndex3);

			maximum = math.max(maximum, triangle.VertexIndex1);
			maximum = math.max(maximum, triangle.VertexIndex2);
			maximum = math.max(maximum, triangle.VertexIndex3);

			return (minimum, maximum);
		}

		private NativeList<Triangle> GetTileTypeTriangles(byte tileType)
		{
			var result = new NativeList<Triangle>(Allocator.Temp);

			foreach (var triangle in MeshUpdateData.Triangles)
			{
				if (triangle.TileType == tileType)
				{
					result.Add(triangle);
				}
			}

			return result;
		}

		private void WriteVertices()
		{
			var vertices = MeshData.GetVertexData<StreamVertex>();

			for (var i = 0; i < MeshUpdateData.Vertices.Length; i++)
			{
				WriteVertex(i, ref vertices);
			}
		}

		private void WriteVertex(int i, ref NativeArray<StreamVertex> vertices)
		{
			var vertex = MeshUpdateData.Vertices[i];

			var streamVertex = new StreamVertex
			{
				Position = vertex.Position,
				Normal = vertex.Normal,
				TexCoord0 = vertex.TexCoord0
			};

			vertices[i] = streamVertex;
		}

		private void WriteTriangles(ref NativeList<Triangle> triangles,
			ref NativeArray<ushort> indexBufferStream,
			int indexBufferStreamOffset
		)
		{
			for (var index = 0; index < triangles.Length; index++)
			{
				WriteTriangle(index, ref triangles, ref indexBufferStream, indexBufferStreamOffset);
			}
		}

		private void WriteTriangle(int index,
			ref NativeList<Triangle> triangles,
			ref NativeArray<ushort> indexBufferStream,
			int indexBufferStreamOffset)
		{
			var triangle = triangles[index];

			var triangleIndex = indexBufferStreamOffset + index * 3;
			indexBufferStream[triangleIndex] = (ushort)triangle.VertexIndex1;
			indexBufferStream[triangleIndex + 1] = (ushort)triangle.VertexIndex2;
			indexBufferStream[triangleIndex + 2] = (ushort)triangle.VertexIndex3;
		}
	}
}
