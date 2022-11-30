using System;
using System.Linq;
using BoundfoxStudios.CommunityProject.Editor.Tools.Terrain.Extensions;
using BoundfoxStudios.CommunityProject.Editor.Tools.Terrain.UI.Components.TileTypePreviewElement;
using BoundfoxStudios.CommunityProject.Editor.Tools.Terrain.UI.Components.ToolbarDropdownButtonElement;
using UnityEngine.UIElements;

namespace BoundfoxStudios.CommunityProject.Editor.Tools.Terrain.UI.Tools.PaintToolElement
{
	public class PaintToolElement : VisualElement
	{
		private ToolbarDropdownButtonElement _dropdownButton;
		private CommunityProject.Terrain.Terrain _terrain;

		public PaintToolElement()
		{
			this.ClassNameToClassList();
			SetupElements();
		}

		~PaintToolElement()
		{
			_dropdownButton.Search -= Search;
		}

		public CommunityProject.Terrain.Terrain Terrain { set => UpdateTileTypes(value); }

		private void UpdateTileTypes(CommunityProject.Terrain.Terrain terrain)
		{
			_terrain = terrain;

			Search(string.Empty);
		}

		private void SetupElements()
		{
			this.LoadUIByGuid("baf045c4a6d3b499bac917bb073bc97a");

			_dropdownButton = this.Q<ToolbarDropdownButtonElement>();
			_dropdownButton.Search += Search;
			_dropdownButton.Toggle += ToggleTool;
		}

		private void ToggleTool(bool isActive)
		{
			
		}

		private void Search(string value)
		{
			_dropdownButton.Clear();

			if (!_terrain)
			{
				_dropdownButton.ClearAndClose();
				return;
			}

			var filteredItems = _terrain.TileTypes.TileTypes;

			if (!string.IsNullOrWhiteSpace(value))
			{
				filteredItems = _terrain.TileTypes.TileTypes
					.Where(tileType => tileType.name.Contains(value, StringComparison.OrdinalIgnoreCase))
					.ToArray();
			}

			foreach (var tileType in filteredItems)
			{
				var tileTypePreviewElement = new TileTypePreviewElement
				{
					TileType = tileType
				};

				_dropdownButton.Add(tileTypePreviewElement);
			}
		}

		public new class UxmlTraits : BindableElement.UxmlTraits { }

		public new class UxmlFactory : UxmlFactory<PaintToolElement, UxmlTraits> { }
	}
}
