using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace BoundfoxStudios.CommunityProject.Terrain.Core
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
				new NativeArray<VertexAttributeDescriptor>(2, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

			descriptor[0] = new(dimension: 3);
			descriptor[1] = new(VertexAttribute.TexCoord0, dimension: 2);

			var vertexCount = MeshUpdateData.Vertices.Length;
			MeshData.SetVertexBufferParams(vertexCount, descriptor);
			descriptor.Dispose();

			const int verticesPerTriangle = 3;
			var triangleIndexCount = MeshUpdateData.Triangles.Length * verticesPerTriangle;
			MeshData.SetIndexBufferParams(triangleIndexCount, IndexFormat.UInt16);

			MeshData.subMeshCount = 1;
			MeshData.SetSubMesh(0, new(0, triangleIndexCount)
			{
				vertexCount = vertexCount,
				bounds = Bounds
			}, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);
		}

		private void WriteTriangles()
		{
			var triangles = MeshData.GetIndexData<ushort>();
			for (var i = 0; i < MeshUpdateData.Triangles.Length; i++)
			{
				WriteTriangle(i, triangles);
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
				TexCoord0 = vertex.TexCoord0
			};

			vertices[i] = streamVertex;
		}

		private void WriteTriangle(int i, NativeArray<ushort> triangles)
		{
			var triangle = MeshUpdateData.Triangles[i];

			var triangleIndex = i * 3;
			triangles[triangleIndex] = (ushort)triangle.VertexIndex1;
			triangles[triangleIndex + 1] = (ushort)triangle.VertexIndex2;
			triangles[triangleIndex + 2] = (ushort)triangle.VertexIndex3;
		}
	}
}
