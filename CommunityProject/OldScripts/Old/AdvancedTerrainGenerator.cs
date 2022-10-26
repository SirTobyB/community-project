using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class AdvancedTerrainGenerator : MonoBehaviour
	{
		private void OnEnable()
		{
			var meshDataArray = Mesh.AllocateWritableMeshData(1);
			var meshData = meshDataArray[0];

			var vertexAttributes = new NativeArray<VertexAttributeDescriptor>(
				4, Allocator.Temp, NativeArrayOptions.UninitializedMemory
			);

			vertexAttributes[0] = new(dimension: 3);
			vertexAttributes[1] = new(VertexAttribute.Normal, stream: 1);
			vertexAttributes[2] = new(VertexAttribute.Tangent, dimension: 4, stream: 2)
			{
				format = VertexAttributeFormat.Float16
			};
			vertexAttributes[3] = new(VertexAttribute.TexCoord0, dimension: 2, stream: 3)
			{
				format = VertexAttributeFormat.Float16
			};

			meshData.SetVertexBufferParams(4, vertexAttributes);
			vertexAttributes.Dispose();

			var position = meshData.GetVertexData<float3>();
			position[0] = 0;
			position[1] = math.right();
			position[2] = math.up();
			position[3] = new(1, 1, 0);

			var normals = meshData.GetVertexData<float3>(1);
			normals[0] = normals[1] = normals[2] = normals[3] = math.back();

			var h0 = math.half(0);
			var h1 = math.half(1);

			var tangets = meshData.GetVertexData<half4>(2);
			tangets[0] = tangets[1] = tangets[2] = tangets[3] = new(h1, h0, h0, math.half(-1));

			var uvs = meshData.GetVertexData<half2>(3);
			uvs[0] = h0;
			uvs[1] = new(h1, h0);
			uvs[2] = new(h0, h1);
			uvs[3] = new(h1, h1);

			meshData.SetIndexBufferParams(6, IndexFormat.UInt16);
			var triangleIndices = meshData.GetIndexData<ushort>();
			triangleIndices[0] = 0;
			triangleIndices[1] = 2;
			triangleIndices[2] = 1;
			triangleIndices[3] = 1;
			triangleIndices[4] = 2;
			triangleIndices[5] = 3;

			var bounds = new Bounds(new Vector3(0.5f, 0.5f), new Vector3(1, 1));

			meshData.subMeshCount = 1;
			meshData.SetSubMesh(0, new(0, 6)
			{
				bounds = bounds,
				vertexCount = 4
			}, MeshUpdateFlags.DontRecalculateBounds);

			var mesh = new Mesh()
			{
				name = "Procedural Mesh",
				bounds = bounds
			};

			Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
			GetComponent<MeshFilter>().mesh = mesh;
		}
	}
}
