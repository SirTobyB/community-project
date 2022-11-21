using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace BoundfoxStudios.CommunityProject.Terrain.Jobs
{
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
			// TODO: Nested List must be unsafe list
			var bucket = new NativeHashMap<float3, NativeList<PositionedVertex>>(0, Allocator.Temp);

			for (var index = 0; index < vertices.Length; index++)
			{
				var vertex = vertices[index];
				if (!bucket.ContainsKey(vertex.Position))
				{
					var list = new NativeList<PositionedVertex>(Allocator.Temp);
					bucket.Add(vertex.Position, list);
				}

				var vertexList = bucket[vertex.Position];
				vertexList.Add(new()
				{
					Index = index,
					Vertex = vertex
				});
			}

			foreach (var kvp in bucket)
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


			bucket.Dispose();
		}
	}
}
