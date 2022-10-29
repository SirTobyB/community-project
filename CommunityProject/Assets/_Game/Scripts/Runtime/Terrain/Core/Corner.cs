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

		private Corner(CornerDirection cornerDirection) => Index = (byte)cornerDirection;

		public static Corner NorthWest => new(CornerDirection.NorthWest);
		public static Corner NorthEast => new(CornerDirection.NorthEast);
		public static Corner SouthWest => new(CornerDirection.SouthWest);
		public static Corner SouthEast => new(CornerDirection.SouthEast);
	}
}
