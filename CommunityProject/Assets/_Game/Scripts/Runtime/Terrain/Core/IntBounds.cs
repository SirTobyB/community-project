using Unity.Mathematics;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.Core
{
	public struct IntBounds
	{
		private readonly int2 _min;
		private readonly int2 _max;

		public int2 Min => _min;
		public int2 Max => _max;
		public int2 Size { get; }
		public int2 Center { get; }

		public IntBounds(int2 min, int2 max)
		{
			_min = min;
			_max = max;
			Size = max - min;
			Center = Size / 2;
		}

		/*
		public IntBounds Intersection(IntBounds other) =>
			new(
				new(math.max(Min.x, other.Min.x), math.max(Min.y, other.Min.y)),
				new(math.min(Max.x, other.Max.x), math.min(Max.y, other.Max.y))
			);

		public bool Intersects(IntBounds other) =>
			Min.x <= other.Max.x
			&& Max.x >= other.Min.x
			&& Min.y <= other.Max.y
			&& Max.y >= other.Min.y;

		public bool Contains(IntBounds other) => math.all(other.Min >= Min) && math.all(other.Max <= Max);*/

		public override string ToString() => $"IntBounds [Min ({Min}) Max({Max})]";

		public readonly bool Contains(int2 position) =>
			position.x >= _min.x && position.x < _max.x && position.y >= _min.y && position.y < _max.y;
	}
}
