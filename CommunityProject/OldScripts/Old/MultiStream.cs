using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	public struct MultiStream : IMeshStreams
	{
		[NativeDisableContainerSafetyRestriction]
		private NativeArray<float3> _positionsStream;

		[NativeDisableContainerSafetyRestriction]
		private NativeArray<float3> _normalsStream;

		[NativeDisableContainerSafetyRestriction]
		private NativeArray<float4> _tangentsStream;

		[NativeDisableContainerSafetyRestriction]
		private NativeArray<float2> _uvsStream;

		[NativeDisableContainerSafetyRestriction]
		private NativeArray<TriangleUInt16> _triangles;

		public void Setup(Mesh.MeshData meshData, Bounds bounds, int vertexCount, int indexCount)
		{
			var descriptor = new NativeArray<VertexAttributeDescriptor>(
				4, Allocator.Temp, NativeArrayOptions.UninitializedMemory
			);

			descriptor[0] = new(dimension: 3);
			descriptor[1] = new(VertexAttribute.Normal, stream: 1);
			descriptor[2] = new(VertexAttribute.Tangent, dimension: 4, stream: 2);
			descriptor[3] = new(VertexAttribute.TexCoord0, dimension: 2, stream: 3);

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

			_positionsStream = meshData.GetVertexData<float3>();
			_normalsStream = meshData.GetVertexData<float3>(1);
			_tangentsStream = meshData.GetVertexData<float4>(2);
			_uvsStream = meshData.GetVertexData<float2>(3);

			_triangles = meshData.GetIndexData<ushort>().Reinterpret<TriangleUInt16>(2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetVertex(int index, Vertex vertex)
		{
			_positionsStream[index] = vertex.Position;
			_normalsStream[index] = vertex.Normal;
			_tangentsStream[index] = vertex.Tangent;
			_uvsStream[index] = vertex.TexCoord0;
		}


		public void SetTriangle(int index, int3 triangle) => _triangles[index] = triangle;
	}
}
