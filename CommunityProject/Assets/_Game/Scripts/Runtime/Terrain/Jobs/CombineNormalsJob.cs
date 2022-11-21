using System;
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

		private struct PositionedVertex
		{
			public Vertex Vertex;
			public int Index;
		}

		public void Execute()
		{
			var vertices = SurfaceMeshUpdateData.Vertices;
			var positionMap = new NativeHashMap<float3, UnsafeList<PositionedVertex>>(0, Allocator.Temp);

			for (var index = 0; index < vertices.Length; index++)
			{
				var vertex = vertices[index];
				if (!positionMap.ContainsKey(vertex.Position))
				{
					var list = new UnsafeList<PositionedVertex>(1, Allocator.Temp);
					positionMap.Add(vertex.Position, list);
				}

				var vertexList = positionMap[vertex.Position];
				vertexList.Add(new()
				{
					Index = index,
					Vertex = vertex
				});
			}

			foreach (var kvp in positionMap)
			{
				var list = kvp.Value;

				if (list.Length == 1)
				{
					vertices[list[0].Index] = list[0].Vertex;
					continue;
				}

				var combinedNormal = float3.zero;

				foreach (var item in list)
				{
					combinedNormal += item.Vertex.Normal;
				}

				foreach (var item in list)
				{
					var vertex = item.Vertex;
					vertex.Normal = math.normalize(combinedNormal / list.Length);

					vertices[item.Index] = vertex;
				}

				list.Dispose();
			}


			positionMap.Dispose();
		}
	}
}
