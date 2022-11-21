using Unity.Mathematics;

namespace BoundfoxStudios.CommunityProject.Terrain.Tiles
{
	public struct IntBounds
	{
		public static IntBounds Invalid => new(new(-1), new(-1));

		private readonly int2 _min;
		private readonly int2 _max;

		public int2 Min => _min;
		public int2 Max => _max;
		public int2 Size { get; }
		public int2 Center { get; }

		public bool IsValid => math.all(Min >= 0) && math.all(Max >= 0);

		public IntBounds(int2 min, int2 max)
		{
			_min = min;
			_max = max;
			Size = max - min;
			Center = min + Size / 2;
		}

		public override string ToString() => $"IntBounds [Min ({Min}) Max({Max})]";

		public readonly bool Contains(int2 position) =>
			position.x >= _min.x && position.x < _max.x && position.y >= _min.y && position.y < _max.y;
	}
}
