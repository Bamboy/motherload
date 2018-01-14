using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
	static GameManager instance;
	public static GameManager singleton{ get{ return instance; } }



	//[HideInInspector]
	public TiledMap level;


	void Awake()
	{
		if( instance == null )
			instance = this;
		else
			Destroy( this.gameObject );

		new TileLookup(); //Loads tile data
	}
	
}
