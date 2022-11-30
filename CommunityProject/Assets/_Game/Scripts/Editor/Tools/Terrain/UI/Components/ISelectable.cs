using System;

namespace BoundfoxStudios.CommunityProject.Editor.Tools.Terrain.UI.Components
{
	public delegate void SelectHandler(SelectedArgs args);

	public struct SelectedArgs
	{
		public string Name { get; set; }
		public ISelectable Item { get; set; }
	}

	public interface ISelectable
	{
		event SelectHandler Selected;
		void Select();
		void Unselect();
	}
}
