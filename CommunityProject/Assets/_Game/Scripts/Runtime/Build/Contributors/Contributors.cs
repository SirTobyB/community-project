using Newtonsoft.Json;

namespace BoundfoxStudios.CommunityProject.Build.Contributors
{
	/// <summary>
	/// This class abstracts the manifest.json file.
	/// </summary>
	[JsonObject]
	public class Contributors
	{
		public Contributor[] Items { get; private set; }

		[JsonConstructor]
		public Contributors(Contributor[] items)
		{
			Items = items;
		}
	}
}
