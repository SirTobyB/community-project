using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace BoundfoxStudios.CommunityProject.Terrain.Jobs
{
	/// <summary>
	///   This job averages the normals within <see cref="SurfaceMeshUpdateData" />.
	///   Before, the normals of a <see cref="Vertex" /> point in a certain direction calculated by
	///   <see cref="NormalCalculationJob" />.
	///   However, this leads to very hard terrain edges when the terrain is raised.
	///   Therefore, this jobs averages the normals to create a smoother terrain.
	/// </summary>
	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	public struct CombineNormalsJob : IJob
	{
		public NativeMeshUpdateData SurfaceMeshUpdateData;

		private struct CombinedNormalIndicesList
		{
			public UnsafeList<int> Indices;
			public float3 CombinedNormal;
		}

		public void Execute()
		{
			var vertices = SurfaceMeshUpdateData.Vertices;
			var positionMap = new NativeHashMap<float3, CombinedNormalIndicesList>(0, Allocator.Temp);

			for (var index = 0; index < vertices.Length; index++)
			{
				var vertex = vertices[index];
				if (!positionMap.ContainsKey(vertex.Position))
				{
					positionMap.Add(vertex.Position, new()
					{
						Indices = new(1, Allocator.Temp)
					});
				}

				var normalVertexList = positionMap[vertex.Position];
				normalVertexList.CombinedNormal += vertex.Normal;
				normalVertexList.Indices.Add(index);
				positionMap[vertex.Position] = normalVertexList;
			}

			foreach (var kvp in positionMap)
			{
				var normalVertexList = kvp.Value;
				var indices = normalVertexList.Indices;

				if (indices.Length == 1)
				{
					continue;
				}

				foreach (var index in indices)
				{
					var vertex = vertices[index];
					vertex.Normal = math.normalize(normalVertexList.CombinedNormal / indices.Length);

					vertices[index] = vertex;
				}

				indices.Dispose();
			}

			positionMap.Dispose();
		}
	}
}
