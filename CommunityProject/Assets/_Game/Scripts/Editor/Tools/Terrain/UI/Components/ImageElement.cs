using UnityEngine.UIElements;

namespace BoundfoxStudios.CommunityProject.Editor.Tools.Terrain.UI.Components
{
	// For no reason, Unity does not expose the image in UI builder, so we do it ourselves.
	public class ImageElement : Image
	{
		public new class UxmlFactory : UxmlFactory<ImageElement, UxmlTraits>{}
	}
}
