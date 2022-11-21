using System;
using BoundfoxStudios.CommunityProject.Extensions;
using BoundfoxStudios.CommunityProject.Terrain.Jobs.Calculations;
using BoundfoxStudios.CommunityProject.Terrain.Tiles;
using Unity.Mathematics;
using UnityEngine;
using Grid = BoundfoxStudios.CommunityProject.Terrain.Tiles.Grid;

namespace BoundfoxStudios.CommunityProject.Terrain.Utils
{
	public static class TerrainRaycasterUtils
	{
		public static bool RaycastTile(Ray ray, out TerrainRaycastHit terrainRaycastHit, float maxRaycastDistance,
			LayerMask layerMask)
		{
			terrainRaycastHit = default;

			if (!Physics.Raycast(ray, out var hitInfo, maxRaycastDistance, layerMask))
			{
				return false;
			}

			if (!hitInfo.collider.TryGetComponentInParent<Terrain>(out var terrain))
			{
				return false;
			}

			var tilePosition = Grid.WorldToTilePosition(hitInfo.point - terrain.transform.position);
			var tile = terrain.Grid.GetTile(tilePosition);

			terrainRaycastHit = new()
			{
				Tile = tile,
				Terrain = terrain,
				TriangleDirection = null,
				Normal = hitInfo.normal
			};

			return true;
		}

		public static bool RaycastTileTriangle(Ray ray, out TerrainRaycastHit terrainRaycastHit, float maxRaycastDistance,
			LayerMask layerMask)
		{
			if (!RaycastTile(ray, out terrainRaycastHit, maxRaycastDistance, layerMask))
			{
				return false;
			}

			var tile = terrainRaycastHit.Tile;
			var heightStep = terrainRaycastHit.Terrain.HeightStep;
			var northWestCorner = SurfaceCalculations.GetCornerPosition(ref tile, Corner.NorthWest, heightStep);
			var northEastCorner = SurfaceCalculations.GetCornerPosition(ref tile, Corner.NorthEast, heightStep);
			var southEastCorner = SurfaceCalculations.GetCornerPosition(ref tile, Corner.SouthEast, heightStep);
			var southWestCorner = SurfaceCalculations.GetCornerPosition(ref tile, Corner.SouthWest, heightStep);
			var center = terrainRaycastHit.Tile.BottomCenter +
			             new float3(0, terrainRaycastHit.Tile.CenterHeight * terrainRaycastHit.Terrain.HeightStep, 0);

			var hitsNorthTriangle = TriangleIntersection(ray, center, northWestCorner, northEastCorner);
			var hitsEastTriangle = TriangleIntersection(ray, center, northEastCorner, southEastCorner);
			var hitsSouthTriangle = TriangleIntersection(ray, center, southEastCorner, southWestCorner);
			var hitsWestTriangle = TriangleIntersection(ray, center, southWestCorner, northWestCorner);

			if (hitsNorthTriangle)
			{
				terrainRaycastHit.TriangleDirection = Directions.North;
			}

			if (hitsEastTriangle)
			{
				terrainRaycastHit.TriangleDirection = Directions.East;
			}

			if (hitsSouthTriangle)
			{
				terrainRaycastHit.TriangleDirection = Directions.South;
			}

			if (hitsWestTriangle)
			{
				terrainRaycastHit.TriangleDirection = Directions.West;
			}

			return true;
		}

		/// <summary>
		/// Performs a ray-triangle intersection by implementing the Möller-Trumbore ray-triangle intersection algorithm.
		/// See: https://en.wikipedia.org/wiki/Möller–Trumbore_intersection_algorithm
		/// This code here is the optimized version from Möller, see: https://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/raytri/
		/// We're using the Option 1 optimization, that can be found here: https://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/raytri/raytri.c
		/// Look for method "intersect_triangle1".
		/// </summary>
		private static bool TriangleIntersection(Ray ray, float3 p1, float3 p2, float3 p3)
		{
			var origin = new float3(ray.origin);
			var direction = new float3(ray.direction);

			var edge1 = p2 - p1;
			var edge2 = p3 - p1;

			var p = math.cross(direction, edge2);
			var det = math.dot(edge1, p);

			if (det > float.Epsilon)
			{
				var t = origin - p1;
				var u = math.dot(t, p);

				if (u is < 0 or > 1)
				{
					return false;
				}

				var q = math.cross(t, edge1);
				var v = math.dot(direction, q);

				if (v < 0 || u + v > det)
				{
					return false;
				}
			}
			else if (det < -float.Epsilon)
			{
				var t = origin - p1;
				var u = math.dot(t, p);

				if (u > 0 || u < det)
				{
					return false;
				}

				var q = math.cross(t, edge1);
				var v = math.dot(direction, q);

				if (v > 0 || u + v < det)
				{
					return false;
				}
			}
			else
			{
				return false;
			}

			return true;
		}
	}
}
