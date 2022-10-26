using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	public struct SingleStream : IMeshStreams
	{
		[StructLayout(LayoutKind.Sequential)]
		private struct Stream0
		{
			public float3 Position;
			public float3 Normal;
			public float4 Tangent;
			public float2 TexCoord0;
		}

		[NativeDisableContainerSafetyRestriction]
		private NativeArray<Stream0> _stream0;

		[NativeDisableContainerSafetyRestriction]
		private NativeArray<TriangleUInt16> _triangles;

		public void Setup(Mesh.MeshData meshData, Bounds bounds, int vertexCount, int indexCount)
		{
			var descriptor = new NativeArray<VertexAttributeDescriptor>(
				4, Allocator.Temp, NativeArrayOptions.UninitializedMemory
			);

			descriptor[0] = new(dimension: 3);
			descriptor[1] = new(VertexAttribute.Normal);
			descriptor[2] = new(VertexAttribute.Tangent, dimension: 4);
			descriptor[3] = new(VertexAttribute.TexCoord0, dimension: 2);

			meshData.SetVertexBufferParams(vertexCount, descriptor);
			descriptor.Dispose();

			meshData.SetIndexBufferParams(indexCount, IndexFormat.UInt16);

			meshData.subMeshCount = 1;
			meshData.SetSubMesh(0, new(0, indexCount)
				{
					bounds = bounds,
					vertexCount = vertexCount
				},
				MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);

			_stream0 = meshData.GetVertexData<Stream0>();

			_triangles = meshData.GetIndexData<ushort>().Reinterpret<TriangleUInt16>(2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetVertex(int index, Vertex vertex) =>
			_stream0[index] = new()
			{
				Position = vertex.Position,
				Normal = vertex.Normal,
				Tangent = vertex.Tangent,
				TexCoord0 = vertex.TexCoord0
			};


		public void SetTriangle(int index, int3 triangle) => _triangles[index] = triangle;
	}
}
