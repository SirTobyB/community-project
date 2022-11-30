using System;
using BoundfoxStudios.CommunityProject.Editor.Tools.Terrain.Extensions;
using UnityEngine.UIElements;

namespace BoundfoxStudios.CommunityProject.Editor.Tools.Terrain.UI.Components.ToggleButtonElement
{
	public class ToggleButtonElement : VisualElement, IToggleable
	{
		private const string IsSelected = "is-selected";
		private Button _button;
		private bool _isSelected;

		public ToggleButtonElement()
		{
			this.ClassNameToClassList();
			SetupElements();
		}

		public string Text { get => _button.text; set => _button.text = value; }

		public event ToggleHandler Toggle = delegate { };

		~ToggleButtonElement()
		{
			_button.clicked -= ButtonClicked;
		}

		private void SetupElements()
		{
			this.LoadUIByGuid("65bbea74a345447beb7d1a6313606269");

			_button = this.Q<Button>();
			_button.clicked += ButtonClicked;
		}

		private void ButtonClicked()
		{
			switch (_isSelected)
			{
				case true:
					_isSelected = false;
					RemoveFromClassList(IsSelected);
					break;
				case false:
					_isSelected = true;
					AddToClassList(IsSelected);
					break;
			}

			Toggle(_isSelected);
		}

		public new class UxmlFactory : UxmlFactory<ToggleButtonElement, UxmlTraits> { }

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

				if (ve is not ToggleButtonElement element)
				{
					return;
				}

				element.Text = _text.GetValueFromBag(bag, cc);
			}
		}
	}
}
