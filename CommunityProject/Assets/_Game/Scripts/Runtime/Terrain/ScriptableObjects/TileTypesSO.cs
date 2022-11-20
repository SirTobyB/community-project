using System;
using System.Collections.Generic;
using System.Linq;
using BoundfoxStudios.CommunityProject.Infrastructure.ScriptableObjects;
using UnityEngine;

namespace BoundfoxStudios.CommunityProject.Terrain.ScriptableObjects
{
	[CreateAssetMenu(menuName = Constants.MenuNames.Terrain + "/Tile Types")]
	public class TileTypesSO : IdentifiableSO
	{
		public TileTypeSO[] TileTypes;

		public byte Length => (byte)TileTypes.Length;

		private Dictionary<byte, TileTypeSO> _tileTypeCache;

		private void OnEnable()
		{
			_tileTypeCache = TileTypes
				.Select((tileType, index) => new
				{
					Index = index,
					TileType = tileType
				})
				.ToDictionary(key => (byte)key.Index, value => value.TileType);
		}

		public TileTypeSO ById(byte id) => _tileTypeCache[id];
	}
}
