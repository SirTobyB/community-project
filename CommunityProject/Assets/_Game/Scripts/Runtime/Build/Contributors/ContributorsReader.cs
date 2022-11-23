using System;
using BoundfoxStudios.CommunityProject.Infrastructure.FileManagement;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BoundfoxStudios.CommunityProject.Build.Contributors
{
	/// <summary>
	/// This class can read the contributors.json.
	/// </summary>
	public class ContributorsReader
	{
		private const string ContributorsAddressablesKey = "Build/contributors.json";

		public async UniTask<Contributor[]> LoadAsync()
		{
			var textAsset = await Addressables.LoadAssetAsync<TextAsset>(ContributorsAddressablesKey);
			var contributors = JsonConvert.DeserializeObject<Contributors>(textAsset.text, JsonFileManager.DefaultSerializerSettings);

			return contributors != null ? contributors.Items : Array.Empty<Contributor>();
		}
	}
}
