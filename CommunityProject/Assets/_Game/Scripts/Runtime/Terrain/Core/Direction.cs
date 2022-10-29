using System;
using Unity.Mathematics;

namespace BoundfoxStudios.CommunityProject.Terrain.Core
{
	public struct Direction
	{
		internal byte Index { get; }

		private static readonly int2[] Vectors = {
			new(0, 1),  // North
			new(1, 0),  // East
			new(0, -1), // South
			new(-1, 0)  // West
		};

		private Direction(byte index)
		{
			if (index > 3)
			{
				throw new ArgumentOutOfRangeException(nameof(index), "Must be 0-4");
			}

			Index = index;
		}

		public static Direction North => new(0);
		public static Direction East => new(1);
		public static Direction South => new(2);
		public static Direction West => new(3);

		public int2 ToVector() => Vectors[Index];
	}
}
