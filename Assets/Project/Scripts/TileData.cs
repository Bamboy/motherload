using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Defines a tile's properties.
/// </summary>
[CreateAssetMenu(fileName="tile", menuName="Tile Definition")]
public class TileData : ScriptableObject
{
	public byte id = 0;

	public string name = "tile";


	public Vector2 texureCoord;


	public override string ToString ()
	{
		return string.Format ("[TileData:{0}] {1}", this.id, this.name);
	}
}
