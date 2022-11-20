using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Extensions
{
	public static class GameObjectExtensions
	{
		public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component)
		{
			component = gameObject.GetComponentInParent<T>();
			return component != null;
		}

		public static bool TryGetComponentInParent<T>(this Collider collider, out T component)
			=> collider.gameObject.TryGetComponentInParent(out component);
	}
}
