using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class AdvancedSingleStreamTerrainGenerator : MonoBehaviour
	{
		private void OnEnable()
		{
			var meshDataArray = Mesh.AllocateWritableMeshData(1);
			var meshData = meshDataArray[0];

			var vertexAttributes = new NativeArray<VertexAttributeDescriptor>(
				4, Allocator.Temp, NativeArrayOptions.UninitializedMemory
			);

			vertexAttributes[0] = new(dimension: 3);
			vertexAttributes[1] = new(VertexAttribute.Normal);
			vertexAttributes[2] = new(VertexAttribute.Tangent, dimension: 4)
			{
				format = VertexAttributeFormat.Float16
			};
			vertexAttributes[3] = new(VertexAttribute.TexCoord0, dimension: 2)
			{
				format = VertexAttributeFormat.Float16
			};

			meshData.SetVertexBufferParams(4, vertexAttributes);
			vertexAttributes.Dispose();

			var vertices = meshData.GetVertexData<Vertex>();

			var h0 = math.half(0);
			var h1 = math.half(1);

			var vertex = new Vertex
			{
				Normal = math.back(),
				Tangent = new(h1, h0, h0, math.half(-1)),
				Position = 0,
				TexCoord0 = h0
			};

			vertices[0] = vertex;

			vertex.Position = math.right();
			vertex.TexCoord0 = new(h1, h0);
			vertices[1] = vertex;

			vertex.Position = math.up();
			vertex.TexCoord0 = new(h0, h1);
			vertices[2] = vertex;

			vertex.Position = new(1, 1, 0);
			vertex.TexCoord0 = new(h1, h1);
			vertices[3] = vertex;

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
