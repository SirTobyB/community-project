using UnityEngine;
using UnityEngine.InputSystem;

namespace BoundfoxStudios.CommunityProject.Input.ScriptableObjects
{
	[CreateAssetMenu(menuName = Constants.MenuNames.Input + "/Input Reader")]
	public class InputReaderSO : ScriptableObject, GameInput.IGameplayActions
	{
		public delegate void SelectEventHandler(Vector2 position);

		/// <summary>
		/// Raised when the player performs a selection, e.g. clicking with the mouse button.
		/// </summary>
		public event SelectEventHandler Select = delegate { };

		public delegate void PositionEventHandler(Vector2 position);

		/// <summary>
		/// Raised when the player moves his pointing device, e.g. moving the mouse.
		/// </summary>
		public event PositionEventHandler Position = delegate {  };

		private GameInput _gameInput;

		private void OnEnable()
		{
			if (_gameInput == null)
			{
				_gameInput = new();

				_gameInput.Gameplay.SetCallbacks(this);
			}
		}

		private void OnDisable()
		{
			DisableAllInput();
		}

		public void DisableAllInput()
		{
			_gameInput.Gameplay.Disable();
			_gameInput.UI.Disable();
		}

		public void EnableGameplayInput()
		{
			_gameInput.UI.Disable();
			_gameInput.Gameplay.Enable();
		}

		public void EnableUIInput()
		{
			_gameInput.Gameplay.Disable();
			_gameInput.UI.Enable();
		}

		public void OnSelect(InputAction.CallbackContext context)
		{
			if (!context.performed)
			{
				return;
			}

			Select(context.ReadValue<Vector2>());
		}

		public void OnPointerPosition(InputAction.CallbackContext context)
		{
			if (!context.performed)
			{
				return;
			}

			Position(context.ReadValue<Vector2>());
		}
	}
}
