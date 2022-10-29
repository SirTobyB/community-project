using System;

namespace BoundfoxStudios.CommunityProject.Terrain.Core
{
	public struct Corner
	{
		internal byte Index { get; }

		public CornerDirection Direction =>
			Index switch
			{
				0 => CornerDirection.NorthWest,
				1 => CornerDirection.NorthEast,
				2 => CornerDirection.SouthEast,
				3 => CornerDirection.SouthWest,
				_ => throw new ArgumentOutOfRangeException(nameof(Index), $"{nameof(Index)} can only be in range 0-3")
			};

		private const int NeighbourClockwise = 1;
		private const int NeighbourOpposite = 2;
		private const int NeighbourCounterClockwise = 3;

		private Corner(CornerDirection cornerDirection) => Index = (byte)cornerDirection;
		private Corner(int index) => Index = (byte)(index % 4);

		public static Corner NorthWest => new(CornerDirection.NorthWest);
		public static Corner NorthEast => new(CornerDirection.NorthEast);
		public static Corner SouthWest => new(CornerDirection.SouthWest);
		public static Corner SouthEast => new(CornerDirection.SouthEast);
		public Corner NeighborCounterClockwise => new(Index + NeighbourCounterClockwise);
		public Corner NeighborClockwise => new(Index + NeighbourClockwise);
		public Corner NeighborOpposite => new(Index + NeighbourOpposite);
	}
}
