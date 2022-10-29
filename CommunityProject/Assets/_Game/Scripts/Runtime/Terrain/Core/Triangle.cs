using Unity.Mathematics;

namespace BoundfoxStudios.CommunityProject.Terrain.Core
{
	public struct Triangle
	{
		public int VertexIndex1 { get; }
		public int VertexIndex2 { get; }
		public int VertexIndex3 { get; }

		public Triangle(int vertexIndex1, int vertexIndex2, int vertexIndex3)
		{
			VertexIndex1 = vertexIndex1;
			VertexIndex2 = vertexIndex2;
			VertexIndex3 = vertexIndex3;
		}
	}
}
