using System;
using BoundfoxStudios.CommunityProject.Editor.Tools.Terrain.Extensions;
using BoundfoxStudios.CommunityProject.Editor.Tools.Terrain.UI.Components.ToolbarDropdownButtonElement;
using BoundfoxStudios.CommunityProject.Editor.Tools.Terrain.UI.Tools.PaintToolElement;
using BoundfoxStudios.CommunityProject.Terrain.ScriptableObjects;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BoundfoxStudios.CommunityProject.Editor.Tools.Terrain.UI
{
	[EditorToolbarElement(Id)]
	public class TerrainToolbar : VisualElement
	{
		public const string Id = "terrain-toolbar";

		protected override Vector2 DoMeasure(float desiredWidth, MeasureMode widthMode, float desiredHeight,
			MeasureMode heightMode) => new(200, 200);

		private PaintToolElement _paintTool;
		private SerializedObject _serializedTileTypes;
		private SerializedObject _serializedTerrain;

		public TerrainToolbar()
		{
			SetupElements();

			SetEnabled(TryGetTerrain(out _));

			Selection.selectionChanged += SelectionChanged;
		}

		private void SetupElements()
		{
			this.LoadUIByGuid("19d114c6a2abd4eff92719fa6d6eecac");
			_paintTool = this.Q<PaintToolElement>();
		}

		~TerrainToolbar()
		{
			Selection.selectionChanged -= SelectionChanged;
		}

		private void SelectionChanged()
		{
			var isSelected = TryGetTerrain(out var terrain);

			SetEnabled(isSelected);
			ResetUi();

			if (!isSelected)
			{
				return;
			}

			UpdateUi(terrain);
		}

		private void UpdateUi(CommunityProject.Terrain.Terrain terrain)
		{
			_paintTool.Terrain = terrain;
		}

		private void ResetUi()
		{
			_paintTool.Terrain = null;
		}

		private bool TryGetTerrain(out CommunityProject.Terrain.Terrain terrain)
		{
			terrain = null;
			return Selection.count == 1 && Selection.activeGameObject &&
			       Selection.activeGameObject.TryGetComponent(out terrain);
		}
	}
}
