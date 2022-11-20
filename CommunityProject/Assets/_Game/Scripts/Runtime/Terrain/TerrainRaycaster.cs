using BoundfoxStudios.CommunityProject.Input.ScriptableObjects;
using BoundfoxStudios.CommunityProject.Terrain.Utils;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	[AddComponentMenu(Constants.MenuNames.Terrain + "/" + nameof(TerrainRaycaster))]
	public class TerrainRaycaster : MonoBehaviour
	{
		[SerializeField]
		private Camera Camera;

		[SerializeField]
		private LayerMask TerrainLayerMask;

		[SerializeField]
		private float MaxRaycastDistance = 10;

		[SerializeField]
		private InputReaderSO InputReader;

		private TerrainSelection _selection;

		public delegate void SelectionChangeEventHandler(TerrainSelection selection);
		public event SelectionChangeEventHandler SelectionChange = delegate { };

		private void OnEnable()
		{
			InputReader.Position += ReadPosition;

			// TODO: Remove, only for testing
			InputReader.EnableGameplayInput();
		}


		private void OnDisable()
		{
			InputReader.Position -= ReadPosition;
		}

		private void ReadPosition(Vector2 position)
		{
			UpdateSelection(position);
		}

		private void UpdateSelection(Vector2 position)
		{
			var ray = Camera.ScreenPointToRay(position);

			if (!TerrainRaycasterUtils.Raycast(ray, out var hitInfo, MaxRaycastDistance, TerrainLayerMask))
			{
				_selection.Clear();
				OnSelectionChange();
				return;
			}

			// TODO: Later, we can use the selected bounds to select more than one tile
			_selection.Terrain = hitInfo.Terrain;
			_selection.Bounds = new(hitInfo.Tile.Position, hitInfo.Tile.Position + 1);
			OnSelectionChange();
		}

		private void OnSelectionChange() => SelectionChange(_selection);
	}
}
