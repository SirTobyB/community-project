using Unity.Mathematics;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	public struct SquareGrid : IMeshGenerator
	{
		public int VertexCount => 4 * Resolution * Resolution;
		public int IndexCount => 6 * Resolution * Resolution;
		public int JobLength => Resolution;
		public int Resolution { get; set; }

		public Bounds Bounds => new(Vector3.zero, new(1, 0, 1));

		public void Execute<TStreams>(int z, TStreams streams) where TStreams : struct, IMeshStreams
		{
			// 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			//             ^  ^  ^  ^

			var vertexIndex = 4 * Resolution * z;
			var triangleIndex = 2 * Resolution * z;

			for (var x = 0; x < Resolution; x++, vertexIndex += 4, triangleIndex += 2)
			{
				var xCoordinate = new float2(x, x + 1f) / Resolution - 0.5f;
				var zCoordinate = new float2(z, z + 1f) / Resolution - 0.5f;

				var vertex = new Vertex();
				vertex.Normal.y = 1;
				vertex.Tangent.xw = new(1, -1);
				vertex.Position.x = xCoordinate.x;
				vertex.Position.z = zCoordinate.x;
				streams.SetVertex(vertexIndex, vertex);

				vertex.Position.x = xCoordinate.y;
				vertex.TexCoord0 = new(1, 0);
				streams.SetVertex(vertexIndex + 1, vertex);

				vertex.Position.x = xCoordinate.x;
				vertex.Position.z = zCoordinate.y;
				vertex.TexCoord0 = new(0, 1);
				streams.SetVertex(vertexIndex + 2, vertex);

				vertex.Position.x = xCoordinate.y;
				vertex.TexCoord0 = new(1, 1);
				streams.SetVertex(vertexIndex + 3, vertex);

				streams.SetTriangle(triangleIndex, vertexIndex + new int3(0, 2, 1));
				streams.SetTriangle(triangleIndex + 1, vertexIndex + new int3(1, 2, 3));
			}
		}
	}
}
