using System.Linq;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
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
			WriteTriangles();
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

			var triangleIndexCount = CalculateTriangleIndexCount();
			MeshData.SetIndexBufferParams(triangleIndexCount, IndexFormat.UInt16);

			var subMeshCount = MeshUpdateData.TileTypeTriangles.Count();
			MeshData.subMeshCount = subMeshCount;

			var previousStartIndex = 0;
			for (var i = 0; i < subMeshCount; i++)
			{
				var triangleBucket = MeshUpdateData.TileTypeTriangles[(byte)i];

				var subMeshTriangleStartIndex = previousStartIndex;
				var subMeshTriangleIndexCount = CalculateTriangleIndexCount(triangleBucket);
				var subMeshVertexCount = subMeshTriangleIndexCount / 3;

				MeshData.SetSubMesh(i, new(subMeshTriangleStartIndex, subMeshTriangleIndexCount)
				{
					vertexCount = subMeshVertexCount,
					bounds = Bounds,
				}, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);

				previousStartIndex += subMeshTriangleIndexCount;
			}
		}

		private int CalculateTriangleIndexCount()
		{
			var result = 0;

			foreach (var kvp in MeshUpdateData.TileTypeTriangles)
			{
				result += CalculateTriangleIndexCount(kvp.Value);
			}

			return result;
		}

		private int CalculateTriangleIndexCount(UnsafeList<Triangle> triangles)
		{
			const int verticesPerTriangle = 3;
			return triangles.Length * verticesPerTriangle;
		}

		private void WriteTriangles()
		{
			var triangles = MeshData.GetIndexData<ushort>();
			var index = 0;

			foreach (var kvp in MeshUpdateData.TileTypeTriangles)
			{
				for (var i = 0; i < kvp.Value.Length; i++)
				{
					WriteTriangle(kvp.Key, i, index, triangles);
					index++;
				}
			}
		}

		private void WriteVertices()
		{
			var vertices = MeshData.GetVertexData<StreamVertex>();
			for (var i = 0; i < MeshUpdateData.Vertices.Length; i++)
			{
				WriteVertex(i, vertices);
			}
		}

		private void WriteVertex(int i, NativeArray<StreamVertex> vertices)
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

		private void WriteTriangle(byte tileType, int i, int index, NativeArray<ushort> triangles)
		{
			var triangle2 = MeshUpdateData.TileTypeTriangles[tileType];
			var triangle = triangle2[i];

			var triangleIndex = index * 3;
			triangles[triangleIndex] = (ushort)triangle.VertexIndex1;
			triangles[triangleIndex + 1] = (ushort)triangle.VertexIndex2;
			triangles[triangleIndex + 2] = (ushort)triangle.VertexIndex3;
		}
	}
}
