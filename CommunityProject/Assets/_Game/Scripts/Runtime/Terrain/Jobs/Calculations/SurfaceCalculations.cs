using System.Runtime.CompilerServices;
using BoundfoxStudios.CommunityProject.Terrain.Tiles;
using Unity.Mathematics;

namespace BoundfoxStudios.CommunityProject.Terrain.Jobs.Calculations
{
	/// <summary>
	///   Contains helper methods used for calculation world positions.
	/// </summary>
	public static class SurfaceCalculations
	{
		/// <summary>
		///   Calculates the world position of a corner.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 GetCornerPosition(ref Tile tile, Corner corner, float heightStep)
			=> tile.GetCornerPosition(corner) * new float3(1, heightStep, 1);

		/// <summary>
		///   Calculates the world position of the center.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 GetCenterPosition(ref Tile tile, float heightStep) =>
			tile.BottomCenter + new float3(0, tile.CenterHeight * heightStep, 0);
	}
}
