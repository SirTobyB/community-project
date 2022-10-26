namespace BoundfoxStudios.CommunityProject.Terrain.Core
{
	public struct Corner
	{
		internal byte Index { get; }

		private Corner(CornerDirection cornerDirection) => Index = (byte)cornerDirection;

		public static Corner NorthWest => new(CornerDirection.NorthWest);
		public static Corner NorthEast => new(CornerDirection.NorthEast);
		public static Corner SouthWest => new(CornerDirection.SouthWest);
		public static Corner SouthEast => new(CornerDirection.SouthEast);
	}
}
