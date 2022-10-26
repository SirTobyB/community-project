namespace BoundfoxStudios.CommunityProject.Terrain.Core
{
	public struct TileData
	{
		private Height _cornerNorthWest;
		private Height _cornerNorthEast;
		private Height _cornerSouthEast;
		private Height _cornerSouthWest;

		public TileData(Height height)
		{
			_cornerNorthWest = height;
			_cornerNorthEast = height;
			_cornerSouthEast = height;
			_cornerSouthWest = height;
		}
	}
}
