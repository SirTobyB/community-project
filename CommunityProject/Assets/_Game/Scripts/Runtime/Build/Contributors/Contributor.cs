using Newtonsoft.Json;

namespace BoundfoxStudios.CommunityProject.Build.Contributors
{
	[JsonObject]
	public class Contributor
	{
		/// <summary>
		/// Username as set in the GitHub profile information
		/// </summary>
		public string User { get; private set; }

		/// <summary>
		/// The contributor's GitHub account name
		/// </summary>
		public string GitHubAccount { get; private set; }

		/// <summary>
		/// Full URL to the contributors GitHub profile
		/// </summary>
		public string ProfileUrl => $"https://github.com/{GitHubAccount}";

		/// <summary>
		/// List with contribution types.
		/// Item will be a string of the AllContributors bot: https://allcontributors.org/docs/en/emoji-key
		/// e.g. "audio", "code", "doc", ...
		/// </summary>
		public string[] Contributions { get; private set; }

		[JsonConstructor]
		public Contributor(string user, string gitHubAccount, string[] contributions)
		{
			User = user;
			GitHubAccount = gitHubAccount;
			Contributions = contributions;
		}
	}
}
