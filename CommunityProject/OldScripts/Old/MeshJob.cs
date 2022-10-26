using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	public struct MeshJob<TGenerator, TStream> : IJobFor
		where TGenerator : struct, IMeshGenerator
		where TStream : struct, IMeshStreams
	{
		private TGenerator _generator;

		[WriteOnly]
		private TStream _streams;

		public void Execute(int index) => _generator.Execute(index, _streams);

		public static JobHandle ScheduleParallel(
			Mesh mesh,
			Mesh.MeshData meshData,
			int resolution,
			JobHandle dependency
		)
		{
			var job = new MeshJob<TGenerator, TStream>();
			job._generator.Resolution = resolution;
			mesh.bounds = job._generator.Bounds;
			job._streams.Setup(meshData, mesh.bounds, job._generator.VertexCount, job._generator.IndexCount);

			return job.ScheduleParallel(job._generator.JobLength, 1, dependency);
		}
	}
}
