using BoundfoxStudios.CommunityProject.Editor.Tools.Terrain.Extensions;
using BoundfoxStudios.CommunityProject.Terrain.ScriptableObjects;
using UnityEditor;
using UnityEngine.UIElements;

namespace BoundfoxStudios.CommunityProject.Editor.Tools.Terrain.UI.Components.TileTypePreviewElement
{
	public class TileTypePreviewElement : VisualElement, ISelectable
	{
		private const string IsSelected = "is-selected";
		private Label _text;
		private Image _image;
		private TileTypeSO _tileType;

		public TileTypePreviewElement()
		{
			this.ClassNameToClassList();
			SetupElements();
		}

		~TileTypePreviewElement()
		{
			UnregisterCallback<ClickEvent>(TileTypeClicked);
		}

		private void SetupElements()
		{
			this.LoadUIByGuid("6589e4ad95df4466e95f93d3d473e379");

			_image = this.Q<ImageElement>();
			_text = this.Q<Label>();

			RegisterCallback<ClickEvent>(TileTypeClicked);
		}

		private void TileTypeClicked(ClickEvent evt)
		{
			Selected(new()
			{
				Name = _tileType.name,
				Item = this
			});
		}

		public new class UxmlFactory : UxmlFactory<TileTypePreviewElement, UxmlTraits> { }

		public TileTypeSO TileType
		{
			get => _tileType;
			set
			{
				_tileType = value;
				var preview = AssetPreview.GetAssetPreview(_tileType.SurfaceMaterial);
				_image.image = preview;
				_text.text = _tileType.name;
			}
		}

		public event SelectHandler Selected = delegate { };
		public void Select()
		{
			AddToClassList(IsSelected);
		}

		public void Unselect()
		{
			RemoveFromClassList(IsSelected);
		}
	}
}
