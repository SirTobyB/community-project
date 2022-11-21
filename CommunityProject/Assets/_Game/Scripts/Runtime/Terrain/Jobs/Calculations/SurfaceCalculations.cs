using System.Runtime.CompilerServices;
using BoundfoxStudios.CommunityProject.Terrain.Tiles;
using Unity.Mathematics;

namespace BoundfoxStudios.CommunityProject.Terrain.Jobs.Calculations
{
	public static class SurfaceCalculations
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 GetCornerPosition(ref Tile tile, Corner corner, float heightStep)
			=> tile.GetCornerPosition(corner) * new float3(1, heightStep, 1);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 GetCenterPosition(ref Tile tile, float heightStep) =>
			tile.BottomCenter + new float3(0, tile.CenterHeight * heightStep, 0);

	}
}
