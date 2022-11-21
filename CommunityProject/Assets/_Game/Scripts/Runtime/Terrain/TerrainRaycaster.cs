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

		[SerializeField]
		private bool DEBUG_USE_TRIANGLE_SELECTION_MODE;

		public delegate void SelectionChangeEventHandler(TerrainSelection? possibleSelection);

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
			TerrainRaycastHit hitInfo = new();

			if ((DEBUG_USE_TRIANGLE_SELECTION_MODE &&
			     !TerrainRaycasterUtils.RaycastTileTriangle(ray, out hitInfo, MaxRaycastDistance, TerrainLayerMask))
			    || (!DEBUG_USE_TRIANGLE_SELECTION_MODE && !TerrainRaycasterUtils.RaycastTile(ray, out hitInfo, MaxRaycastDistance, TerrainLayerMask))
			   )
			{
				OnSelectionChange(null);
				return;
			}

			if (hitInfo.IsWall)
			{
				OnSelectionChange(null);
				return;
			}

			var selection = new TerrainSelection()
			{
				Terrain = hitInfo.Terrain,
			// TODO: Later, we can use the selected bounds to select more than one tile
				Bounds = new(hitInfo.Tile.Position, hitInfo.Tile.Position + 1),
				Triangle = hitInfo.TriangleDirection
			};

			OnSelectionChange(selection);
		}

		private void OnSelectionChange(TerrainSelection? possibleSelection) => SelectionChange(possibleSelection);
	}
}
