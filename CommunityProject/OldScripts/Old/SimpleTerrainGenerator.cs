using System;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class SimpleTerrainGenerator : MonoBehaviour
	{
		private void OnEnable()
		{
			var mesh = new Mesh()
			{
				name = "Procedural Mesh"
			};

			mesh.SetVertices(new[]
			{
				Vector3.zero, Vector3.right, Vector3.up, new Vector3(1, 1),
			});

			mesh.SetTriangles(new[] { 0, 2, 1, 1, 2, 3 }, 0);

			mesh.SetNormals(new[]
			{
				Vector3.back, Vector3.back, Vector3.back, Vector3.back
			});

			mesh.SetTangents(new[]
			{
				new Vector4(1f, 0f, 0f, -1f),
				new Vector4(1f, 0f, 0f, -1f),
				new Vector4(1f, 0f, 0f, -1f),
				new Vector4(1f, 0f, 0f, -1f),
			});

			mesh.SetUVs(0, new[]
			{
				Vector2.zero, Vector2.right, Vector2.up, Vector2.one,
			});

			GetComponent<MeshFilter>().mesh = mesh;
		}
	}
}
