using BoundfoxStudios.CommunityProject.Input.ScriptableObjects;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.Diagnostics
{
	[AddComponentMenu(Constants.MenuNames.Terrain + "/" + nameof(TerrainRaycasterDebugger))]
	public class TerrainRaycasterDebugger : MonoBehaviour
	{
		[SerializeField]
		private TerrainRaycaster Raycaster;

		[SerializeField]
		private Camera Camera;

		[SerializeField]
		private LayerMask TerrainLayerMask;

		[SerializeField]
		private float MaxRaycastDistance = 10;

		[SerializeField]
		private InputReaderSO InputReader;

		private void OnEnable()
		{
			InputReader.Click += ReadClick;

			InputReader.EnableGameplayInput();
		}

		private void OnDisable()
		{
			InputReader.Click -= ReadClick;
		}

		private void ReadClick(Vector2 position)
		{
			var ray = Camera.ScreenPointToRay(position);

			if (Raycaster.Raycast(ray, out var hitInfo, MaxRaycastDistance, TerrainLayerMask))
			{
				Debug.Log($"Tile Position {hitInfo.Tile.Position}");
				return;
			}

			Debug.Log("No hit");
		}
	}
}
