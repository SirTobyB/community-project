using System;
using BoundfoxStudios.CommunityProject.Infrastructure.FileManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Settings.ScriptableObjects
{
	/// <summary>
	///   Holding information about game settings.
	/// </summary>
	[CreateAssetMenu(fileName = "GameSettings", menuName = Constants.MenuNames.MenuName + "/GameSettings")]
	public class SettingsSO : ScriptableObject
	{
		private readonly string _jsonFileName = "config.json";
		private GameSettings _gameSettings;

		private JsonFileManager _jsonFileManager;

		public AudioConfig Audio => _gameSettings.Audio;
		public GraphicConfig Graphic => _gameSettings.Graphic;

		private void OnEnable()
		{
			_jsonFileManager = new();
		}

		public async UniTask SaveAsync()
		{
			await _jsonFileManager.WriteAsync(_jsonFileName, _gameSettings);
		}

		public async UniTask LoadAsync()
		{
			var fileExists = await _jsonFileManager.ExistsAsync(_jsonFileName);

			if (!fileExists)
			{
				return;
			}

			_gameSettings = await _jsonFileManager.ReadAsync<GameSettings>(_jsonFileName);
		}

		[Serializable]
		public class GameSettings
		{
			public AudioConfig Audio;
			public GraphicConfig Graphic;
		}

		[Serializable]
		public class AudioConfig
		{
			[Range(0f, 1f)]
			public float EffectsVolume = 1f;

			[Range(0f, 1f)]
			public float MasterVolume = 1f;

			[Range(0f, 1f)]
			public float MusicVolume = 1f;

			[Range(0f, 1f)]
			public float UIVolume = 1f;
		}

		[Serializable]
		public class GraphicConfig
		{
			public int GraphicLevel = 1;
			public bool IsFullscreen = true;
			public int ResolutionIndex = -1;
		}
	}
}
