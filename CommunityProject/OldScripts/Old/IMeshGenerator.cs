using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	public interface IMeshGenerator
	{
		int VertexCount { get; }
		int IndexCount { get; }
		int JobLength { get; }
		Bounds Bounds { get; }
		int Resolution { get; set; }

		void Execute<TStreams>(int z, TStreams streams) where TStreams : struct, IMeshStreams;
	}
}
