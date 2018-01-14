using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Represents a physical tile located in the map.
/// </summary>
[System.Serializable]
public struct MapTile
{

	public byte type;

	public bool isSolid;

	public int x;
	public int y;

}
