namespace BoundfoxStudios.CommunityProject.Terrain.Jobs
{
	/// <summary>
	/// Data structure to describe a triangle.
	/// </summary>
	public struct Triangle
	{
		public int VertexIndex1 { get; }
		public int VertexIndex2 { get; }
		public int VertexIndex3 { get; }
		public byte TileType { get; }

		public Triangle(byte tileType, int vertexIndex1, int vertexIndex2, int vertexIndex3)
		{
			TileType = tileType;
			VertexIndex1 = vertexIndex1;
			VertexIndex2 = vertexIndex2;
			VertexIndex3 = vertexIndex3;
		}
	}
}
