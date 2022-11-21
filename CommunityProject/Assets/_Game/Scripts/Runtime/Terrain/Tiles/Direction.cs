using System;
using Unity.Mathematics;

namespace BoundfoxStudios.CommunityProject.Terrain.Tiles
{
	public struct Direction
	{
		internal byte Index { get; }

		public Directions ABC => Index switch
		{
			0 => Directions.North,
			1 => Directions.East,
			2 => Directions.South,
			3 => Directions.West,
			_ => throw new ArgumentOutOfRangeException(nameof(Index), $"{nameof(Index)} can only be in range 0-3")
		};

		private static readonly int2[] Vectors = {
			new(0, 1),  // North
			new(1, 0),  // East
			new(0, -1), // South
			new(-1, 0)  // West
		};

		public Direction(Directions direction) => Index = (byte)direction;

		public static Direction North => new(Directions.North);
		public static Direction East => new(Directions.East);
		public static Direction South => new(Directions.South);
		public static Direction West => new(Directions.West);

		public int2 ToVector() => Vectors[Index];
	}
}
