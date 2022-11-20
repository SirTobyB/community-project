using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BoundfoxStudios.CommunityProject.Input.ScriptableObjects
{
	[CreateAssetMenu(menuName = Constants.MenuNames.Input + "/Input Reader")]
	public class InputReaderSO : ScriptableObject, GameInput.IGameplayActions
	{
		public delegate void ClickEventHandler(Vector2 position);
		public event ClickEventHandler Click = delegate { };

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

		public void OnClick(InputAction.CallbackContext context)
		{
			if (!context.performed)
			{
				return;
			}

			Click(context.ReadValue<Vector2>());
		}
	}
}
