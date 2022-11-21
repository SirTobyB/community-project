using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace BoundfoxStudios.CommunityProject.Terrain.Jobs
{
	/// <summary>
	///   This job calculates the normals of the mesh.
	/// </summary>
	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	public struct NormalCalculationJob : IJob
	{
		public NativeMeshUpdateData MeshUpdateData;

		public void Execute()
		{
			var vertices = MeshUpdateData.Vertices;

			CalculateNormals(ref vertices);
			Normalize(ref vertices);
		}

		private void Normalize(ref NativeList<Vertex> vertices)
		{
			for (var i = 0; i < vertices.Length; i++)
			{
				var vertex = vertices[i];
				vertex.Normal = math.normalize(vertex.Normal);
				vertices[i] = vertex;
			}
		}

		private void CalculateNormals(ref NativeList<Vertex> vertices)
		{
			foreach (var triangle in MeshUpdateData.Triangles)
			{
				var vertexA = vertices[triangle.VertexIndex1];
				var vertexB = vertices[triangle.VertexIndex2];
				var vertexC = vertices[triangle.VertexIndex3];

				var triangleNormal = CalculateNormal(vertexA.Position, vertexB.Position, vertexC.Position);

				vertexA.Normal += triangleNormal;
				vertexB.Normal += triangleNormal;
				vertexC.Normal += triangleNormal;

				vertices[triangle.VertexIndex1] = vertexA;
				vertices[triangle.VertexIndex2] = vertexB;
				vertices[triangle.VertexIndex3] = vertexC;
			}
		}

		private float3 CalculateNormal(float3 pointA, float3 pointB, float3 pointC)
		{
			var sideA = pointB - pointA;
			var sideB = pointC - pointA;

			return math.normalize(math.cross(sideA, sideB));
		}
	}
}
