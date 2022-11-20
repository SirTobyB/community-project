using System.Collections.Generic;
using System.Linq;
using BoundfoxStudios.CommunityProject.Terrain.Chunks;
using Unity.Mathematics;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain
{
	[AddComponentMenu(Constants.MenuNames.Terrain + "/" + nameof(TerrainCollider))]
	public class TerrainCollider : MonoBehaviour
	{
		[SerializeField]
		private TerrainRenderer TerrainRenderer;

		private readonly Dictionary<int2, MeshCollider> _colliders = new();

		private GameObject _internalCollidersGameObject;

		private void Awake()
		{
			_internalCollidersGameObject = new("Internal Colliders")
			{
				hideFlags = HideFlags.NotEditable
			};

			_internalCollidersGameObject.transform.SetParent(transform);
		}

		private void OnEnable()
		{
			TerrainRenderer.AfterUpdateChunks += UpdateColliders;
		}

		private void OnDisable()
		{
			TerrainRenderer.AfterUpdateChunks -= UpdateColliders;
		}

		private void UpdateColliders(IReadOnlyCollection<Chunk> chunks)
		{
			foreach (var chunk in chunks)
			{
				UpdateCollider(chunk);
			}
		}

		private void UpdateCollider(Chunk chunk)
		{
			if (!_colliders.TryGetValue(chunk.Position, out var meshColllider))
			{
				meshColllider = _internalCollidersGameObject.AddComponent<MeshCollider>();
				_colliders.Add(chunk.Position, meshColllider);
			}

			var mesh = new Mesh();
			mesh.CombineMeshes(
				CreateCombineInstances(chunk.SurfaceMesh)
					.Concat(CreateCombineInstances(chunk.WallMesh))
					.ToArray(),
				true, true, false);

			meshColllider.sharedMesh = mesh;
		}

		private CombineInstance[] CreateCombineInstances(Mesh mesh)
		{
			var result = new CombineInstance[mesh.subMeshCount];

			for (var i = 0; i < mesh.subMeshCount; i++)
			{
				result[i] = new()
				{
					transform = transform.localToWorldMatrix,
					mesh = mesh,
					subMeshIndex = i
				};
			}

			return result;
		}
	}
}
