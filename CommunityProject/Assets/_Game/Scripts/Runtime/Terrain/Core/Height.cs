namespace BoundfoxStudios.CommunityProject.Terrain.Core
{
	public struct Height
	{
		public byte Value { get; }

		public Height(byte value)
		{
			Value = value;
		}

		public static Height operator /(Height lhs, byte rhs) => new ((byte) (lhs.Value / rhs));
	}
}
