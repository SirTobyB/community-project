using Newtonsoft.Json;

namespace BoundfoxStudios.CommunityProject.Build.BuildManifest
{
	/// <summary>
	/// This class abstracts the manifest.json file.
	/// </summary>
	[JsonObject]
	public class BuildManifest
	{
		public string Sha { get; private set; }
		public string ShortSha { get; private set; }
		public int RunId { get; private set; }
		public int RunNumber { get; private set; }

		[JsonConstructor]
		public BuildManifest(string shortSha, int runId, int runNumber, string sha)
		{
			Sha = sha;
			ShortSha = shortSha;
			RunId = runId;
			RunNumber = runNumber;
		}
	}
}
