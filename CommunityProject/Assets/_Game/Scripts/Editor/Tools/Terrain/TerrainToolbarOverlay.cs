using BoundfoxStudios.CommunityProject.Editor.Tools.Terrain.UI;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;

namespace BoundfoxStudios.CommunityProject.Editor.Tools.Terrain
{
	[Overlay(typeof(SceneView), "terrain", "Terrain")]
	public class TerrainToolbarOverlay : Overlay
	{
		public override VisualElement CreatePanelContent() => new TerrainToolbar();

		protected override Layout supportedLayouts => Layout.Panel;
	}
}
