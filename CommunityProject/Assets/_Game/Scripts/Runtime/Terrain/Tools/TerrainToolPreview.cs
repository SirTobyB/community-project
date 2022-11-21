using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.Tools
{
	[AddComponentMenu(Constants.MenuNames.Terrain + "/" + nameof(TerrainToolPreview))]
	public class TerrainToolPreview : MonoBehaviour
	{
		[SerializeField]
		private TerrainRaycaster Raycaster;

		[SerializeField]
		private Material PreviewMaterial;

		private PreviewMesh _previewMesh;

		private void Awake()
		{
			_previewMesh = new(0.0001f);
		}

		private void OnEnable()
		{
			Raycaster.SelectionChange += UpdatePreviewPosition;
		}

		private void OnDisable()
		{
			Raycaster.SelectionChange -= UpdatePreviewPosition;
		}

		private void UpdatePreviewPosition(TerrainSelection selection)
		{
			_previewMesh.Clear();

			if (!selection.HasSelection)
			{
				return;
			}

			_previewMesh.UpdateMesh(selection.Terrain, selection.Bounds, selection.Triangle);
		}

		private void LateUpdate()
		{
			_previewMesh.Render(transform.localToWorldMatrix, PreviewMaterial, gameObject.layer);
		}
	}
}
