using System;
using BoundfoxStudios.CommunityProject.Editor.Tools.Terrain.Extensions;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BoundfoxStudios.CommunityProject.Editor.Tools.Terrain.UI.Components.ToolbarDropdownButtonElement
{
	public class ToolbarDropdownButtonElement : VisualElement, ISelectable, IToggleable
	{
		private VisualElement _bottomPart;
		private SelectedArgs? _currentSelectedItem;
		private ToggleButtonElement.ToggleButtonElement _primaryButton;
		private ToolbarSearchField _search;
		private Button _toggleButton;
		private string _text;

		public event ToggleHandler Toggle = delegate {  };

		public ToolbarDropdownButtonElement()
		{
			this.ClassNameToClassList();
			SetupElements();
		}

		public ScrollView ItemContainer { get; private set; }

		public override VisualElement contentContainer => ItemContainer;

		public string Text
		{
			get => _text;
			set
			{
				_text = value;
				UpdateText();
			}
		}

		private void UpdateText()
		{
			_primaryButton.Text = $"{_text}{(_currentSelectedItem != null ? $": {_currentSelectedItem.Value.Name}" : "")}";
		}

		public event SelectHandler Selected = delegate { };

		public void Select()
		{
			throw new NotSupportedException();
		}

		public void Unselect()
		{
			foreach (var child in Children())
			{
				if (child is ISelectable selectable)
				{
					selectable.Unselect();
				}
			}
		}

		public event Action<string> Search = delegate { };

		public new void Add(VisualElement child)
		{
			if (child is ISelectable selectable)
			{
				selectable.Selected += ItemSelected;
			}

			base.Add(child);
		}

		private void ItemSelected(SelectedArgs args)
		{
			_currentSelectedItem?.Item.Unselect();
			_currentSelectedItem = args;
			_currentSelectedItem.Value.Item.Select();
			_primaryButton.SetEnabled(_currentSelectedItem != null);
			Close();
			UpdateText();

			Selected(args);
		}

		public new void Clear()
		{
			foreach (var child in Children())
			{
				if (child is ISelectable selectable)
				{
					selectable.Selected -= ItemSelected;
				}
			}

			base.Clear();
		}

		public void ClearAndClose()
		{
			Close();
			Clear();
			_search.value = string.Empty;
		}

		private void Close()
		{
			_bottomPart.style.display = new(DisplayStyle.None);
		}

		private void SetupElements()
		{
			this.LoadUIByGuid("83a8a011b25bf476ea8b92fc0353ab44", true);

			_primaryButton = this.Q<ToggleButtonElement.ToggleButtonElement>("PrimaryButton");
			_primaryButton.SetEnabled(false);
			_primaryButton.Toggle += Toggle;

			_toggleButton = this.Q<Button>("ToggleButton");
			_toggleButton.clicked += ToggleItemContainer;

			_bottomPart = this.Q("Bottom");
			Close();

			ItemContainer = this.Q<ScrollView>();

			_search = this.Q<ToolbarSearchField>();
			_search.RegisterValueChangedCallback(SearchChanged);
		}

		~ToolbarDropdownButtonElement()
		{
			_primaryButton.Toggle -= Toggle;
			_toggleButton.clicked -= ToggleItemContainer;
			_search.UnregisterValueChangedCallback(SearchChanged);
		}

		private void SearchChanged(ChangeEvent<string> evt)
		{
			Search(evt.newValue);
		}

		private void ToggleItemContainer()
		{
			_bottomPart.style.display = new(
				_bottomPart.style.display.value == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None
			);
		}

		public new class UxmlFactory : UxmlFactory<ToolbarDropdownButtonElement, UxmlTraits> { }

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			private readonly UxmlStringAttributeDescription _text = new()
			{
				name = nameof(Text),
				defaultValue = "Text"
			};

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				if (ve is not ToolbarDropdownButtonElement element)
				{
					return;
				}

				element.Text = _text.GetValueFromBag(bag, cc);
			}
		}
	}
}
