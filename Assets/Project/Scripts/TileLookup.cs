using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager class used to look up tile information.
/// </summary>
public class TileLookup 
{
	static TileLookup instance;
	public static TileLookup singleton
	{ 
		get{ 
			if( instance == null )
				instance = new TileLookup();

			return instance; 
		} 
	}

	public static Dictionary<byte, TileData> tiles;

	public TileLookup()
	{
		tiles = new Dictionary<byte, TileData>();

		TileData[] datas = Resources.LoadAll<TileData>("");

		foreach (TileData tile in datas) 
		{
			if( tiles.ContainsKey( tile.id ) )
			{
				Debug.LogError(string.Format("Duplicate tile ids! {0} - {1}", tiles[tile.id].ToString(), tile.ToString()));
				break;
			}

			tiles.Add( tile.id, tile );
		}
	}







}
