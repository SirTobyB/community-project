using System.Linq;
using BoundfoxStudios.CommunityProject.Terrain.ScriptableObjects;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace BoundfoxStudios.CommunityProject.Editor.Editors.Terrain
{
	[CustomEditor(typeof(CommunityProject.Terrain.Terrain))]
	public class TerrainEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var terrain = (CommunityProject.Terrain.Terrain)target;

			ShowTerrainDataNotice(terrain);
		}

		private void ShowTerrainDataNotice(CommunityProject.Terrain.Terrain terrain)
		{
			if (terrain.TerrainData)
			{
				return;
			}

			EditorGUILayout.HelpBox("Caution! This terrain does not have an TerrainData scriptable object set." +
			                        "Any editor playmode modification of the terrain will not be saved.", MessageType.Warning);

			if (GUILayout.Button("Create TerrainDataSO"))
			{
				CreateAndSaveTerrainDataSO(terrain);
			}
		}

		private void CreateAndSaveTerrainDataSO(CommunityProject.Terrain.Terrain terrain)
		{
			const string terrainFolderGuid = "1d5ee6ae2441843a0a26a839375dd9a9";
			var startupPath = AssetDatabase.GUIDToAssetPath(terrainFolderGuid);

			var filePath = EditorUtility.SaveFilePanelInProject(
				"Save TerrainDataSO",
				$"{SceneManager.GetActiveScene().name}_TerrainData",
				"asset",
				"Please enter file name to save TerrainDataSO",
				startupPath
			);

			if (string.IsNullOrWhiteSpace(filePath))
			{
				return;
			}

			var terrainData = CreateInstance<TerrainDataSO>();
			AssetDatabase.CreateAsset(terrainData, filePath);

			Undo.RecordObject(terrain, "Assign TerrainData");
			terrain.TerrainData = terrainData;
			EditorUtility.SetDirty(terrain);
		}
	}
}
