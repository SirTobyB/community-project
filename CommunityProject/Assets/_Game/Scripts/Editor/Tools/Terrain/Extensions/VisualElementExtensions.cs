using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BoundfoxStudios.CommunityProject.Editor.Tools.Terrain.Extensions
{
	public static class VisualElementExtensions
	{
		private static string PascalToKebabCase(this string value)
		{
			if (string.IsNullOrEmpty(value))
				return value;

			return Regex.Replace(
					value,
					"(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z0-9])",
					"-$1",
					RegexOptions.Compiled)
				.Trim()
				.ToLower();
		}

		public static void ClassNameToClassList(this VisualElement element)
		{
			var type = element.GetType();
			element.AddToClassList(type.Name.PascalToKebabCase());
		}

		public static void LoadUIByGuid(this VisualElement element, string guid, bool addToHierarchy = false)
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);

			if (!template)
			{
				Debug.LogWarning($"Did not find {path}");
				return;
			}

			var ui = template.CloneTree();

			if (addToHierarchy)
			{
				element.hierarchy.Add(ui);
				return;
			}

			element.Add(ui);
		}
	}
}
