using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Debug = UnityEngine.Debug;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class ProceduralMesh : MonoBehaviour
	{
		[SerializeField]
		[Range(1, 10)]
		private int Resolution = 1;

		[SerializeField]
		private bool UseMultiStream = true;

		private void UpdateMesh()
		{
			var mesh = GenerateMesh();
			GetComponent<MeshFilter>().mesh = mesh;
		}

		private Mesh GenerateMesh()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var mesh = new Mesh()
			{
				name = "Procedural Mesh"
			};

			var meshDataArray = Mesh.AllocateWritableMeshData(1);
			var meshData = meshDataArray[0];

			if (UseMultiStream)
			{
				Debug.Log("Scheduling MultiStream update");
				MeshJob<SquareGrid, MultiStream>.ScheduleParallel(mesh, meshData, Resolution, default).Complete();
			}
			else
			{
				Debug.Log("Scheduling SingleStream update");
				MeshJob<SquareGrid, SingleStream>.ScheduleParallel(mesh, meshData, Resolution, default).Complete();
			}

			Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);

			stopwatch.Stop();
			Debug.Log($"Time to generate mesh: {stopwatch.ElapsedMilliseconds} msec");

			return mesh;
		}

		private void Update()
		{
			UpdateMesh();
			enabled = false;
		}

		private void OnValidate()
		{
			enabled = true;
		}
	}
}
