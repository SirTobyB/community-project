using System;

namespace BoundfoxStudios.CommunityProject.Terrain.Tiles
{
	public struct Corner
	{
		internal byte Index { get; }

		public Corners Direction =>
			Index switch
			{
				0 => Corners.NorthWest,
				1 => Corners.NorthEast,
				2 => Corners.SouthEast,
				3 => Corners.SouthWest,
				_ => throw new ArgumentOutOfRangeException(nameof(Index), $"{nameof(Index)} can only be in range 0-3")
			};

		private const int NeighbourClockwise = 1;
		private const int NeighbourOpposite = 2;
		private const int NeighbourCounterClockwise = 3;

		private Corner(Corners corners) => Index = (byte)corners;
		private Corner(int index) => Index = (byte)(index % 4);

		public static Corner NorthWest => new(Corners.NorthWest);
		public static Corner NorthEast => new(Corners.NorthEast);
		public static Corner SouthWest => new(Corners.SouthWest);
		public static Corner SouthEast => new(Corners.SouthEast);
		public Corner NeighborCounterClockwise => new(Index + NeighbourCounterClockwise);
		public Corner NeighborClockwise => new(Index + NeighbourClockwise);
		public Corner NeighborOpposite => new(Index + NeighbourOpposite);
	}
}
