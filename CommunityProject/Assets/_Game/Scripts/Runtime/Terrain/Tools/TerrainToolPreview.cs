using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.Tools
{
	[AddComponentMenu(Constants.MenuNames.Terrain + "/" + nameof(TerrainToolPreview))]
	public class TerrainToolPreview : MonoBehaviour
	{
		[SerializeField]
		private TerrainSelection Selection;

		[SerializeField]
		private Material PreviewMaterial;

		private PreviewMesh _previewMesh;

		private void Awake()
		{
			_previewMesh = new(0.0001f);
		}

		private void OnEnable()
		{
			Selection.Change += UpdatePreviewPosition;
		}

		private void OnDisable()
		{
			Selection.Change -= UpdatePreviewPosition;
		}

		private void UpdatePreviewPosition(Selection? possibleSelection)
		{
			_previewMesh.Clear();

			if (possibleSelection is null)
			{
				return;
			}

			var selection = possibleSelection.Value;

			_previewMesh.UpdateMesh(selection.Terrain, selection.Bounds, selection.Triangle);
		}

		private void LateUpdate()
		{
			_previewMesh.Render(transform.localToWorldMatrix, PreviewMaterial, gameObject.layer);
		}
	}
}
