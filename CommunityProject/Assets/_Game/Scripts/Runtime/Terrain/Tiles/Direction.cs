using System;
using Unity.Mathematics;

namespace BoundfoxStudios.CommunityProject.Terrain.Tiles
{
	public struct Direction
	{
		internal byte Index { get; }

		public CardinalDirection CardinalDirection => Index switch
		{
			0 => CardinalDirection.North,
			1 => CardinalDirection.East,
			2 => CardinalDirection.South,
			3 => CardinalDirection.West,
			_ => throw new ArgumentOutOfRangeException(nameof(Index), $"{nameof(Index)} can only be in range 0-3")
		};

		private static readonly int2[] Vectors = {
			new(0, 1),  // North
			new(1, 0),  // East
			new(0, -1), // South
			new(-1, 0)  // West
		};

		public Direction(CardinalDirection cardinalDirection) => Index = (byte)cardinalDirection;

		public static Direction North => new(CardinalDirection.North);
		public static Direction East => new(CardinalDirection.East);
		public static Direction South => new(CardinalDirection.South);
		public static Direction West => new(CardinalDirection.West);

		public int2 Vector => Vectors[Index];
	}
}
