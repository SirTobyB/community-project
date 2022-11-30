using System;

namespace BoundfoxStudios.CommunityProject.Editor.Tools.Terrain.UI.Components
{
	public delegate void ToggleHandler(bool isActive);

	public interface IToggleable
	{
		event ToggleHandler Toggle;
	}
}
