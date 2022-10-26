using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	[StructLayout(LayoutKind.Sequential)]
	public struct TriangleUInt16
	{
		public ushort A;
		public ushort B;
		public ushort C;

		public static implicit operator TriangleUInt16(int3 triangle) => new TriangleUInt16()
		{
			A = (ushort)triangle.x,
			B = (ushort)triangle.y,
			C = (ushort)triangle.z
		};
	}
}
