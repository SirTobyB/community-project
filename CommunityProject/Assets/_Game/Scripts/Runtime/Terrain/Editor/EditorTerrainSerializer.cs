using System;
using UnityEngine;
#if UNITY_EDITOR
using Newtonsoft.Json;
using UnityEditor;
#endif

namespace BoundfoxStudios.CommunityProject.Terrain.Editor
{
	[ExecuteAlways]
	public class EditorTerrainSerializer : MonoBehaviour
	{
#if UNITY_EDITOR
		[SerializeField]
		private Terrain Terrain;

		private void Start()
		{
			if (EditorApplication.isPlaying)
			{
				return;
			}

			LoadTerrainData();
		}

		private void OnDestroy()
		{
			if (!Terrain)
			{
				return;
			}

			Terrain.Grid.Dispose();
		}

		private void OnEnable()
		{
			AssemblyReloadEvents.beforeAssemblyReload += BeforeAssemblyReload;
			AssemblyReloadEvents.afterAssemblyReload += AfterAssemblyReload;
		}

		private void AfterAssemblyReload()
		{
			LoadTerrainData();
		}

		private void OnDisable()
		{
			AssemblyReloadEvents.afterAssemblyReload -= AfterAssemblyReload;
			AssemblyReloadEvents.beforeAssemblyReload -= BeforeAssemblyReload;
		}

		private void BeforeAssemblyReload()
		{
			if (!Terrain)
			{
				return;
			}

			Terrain.Grid.Dispose();
		}

		/*private void SaveTerrainData()
		{
			var container = Terrain.GetDataContainerAsync().GetAwaiter().GetResult();
			Terrain.TerrainData.Data = JsonConvert.SerializeObject(container);

			var path = AssetDatabase.GetAssetPath(Terrain.TerrainData);
			AssetDatabase.ImportAsset(path);
			EditorUtility.SetDirty(Terrain.TerrainData);
			AssetDatabase.SaveAssetIfDirty(Terrain.TerrainData);
		}*/  

		private void LoadTerrainData()
		{
			var json = Terrain.TerrainData.Data;
			var container = JsonConvert.DeserializeObject<Terrain.DataContainer>(json);

			if (container == null)
			{
				return;
			}

			Terrain.SetDataContainerAsync(container).GetAwaiter().GetResult();
		}
#endif
	}
}
