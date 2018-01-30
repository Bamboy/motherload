using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
	static GameManager instance;
	public static GameManager singleton{ get{ return instance; } }

	public GameObject player;

	//[HideInInspector]
	public TiledMap level;


	void Awake()
	{
		if( instance == null )
			instance = this;
		else
		{
			Destroy( this.gameObject );
			return;
		}

		new TileLookup(); //Loads tile data

		inputEnabled = true;
	}


	public static bool inputEnabled{ get; set; }

	public static float inputHoriz
	{
		get{ return inputEnabled ? Input.GetAxis("Horizontal") : 0f; }
	}
	public static float inputVert
	{
		get{ return inputEnabled ? Input.GetAxis("Vertical") : 0f; }
	}
	public static Vector2 inputAxis
	{
		get{
			if( inputEnabled )
				return new Vector2( Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") );
			else 
				return Vector2.zero;
		}
	}

	public static bool inputLeft
	{
		get{ return inputEnabled && (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow)); }
	}
	public static bool inputRight
	{
		get{ return inputEnabled && (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow)); }
	}
	public static bool inputUp
	{
		get{ return inputEnabled && (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow)); }
	}
	public static bool inputDown
	{
		get{ return inputEnabled && (Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.DownArrow)); }
	}

	public static bool inputAny
	{
		get{ return inputLeft || inputRight || inputDown || inputUp; }
	}

	public static Edges InputToEdge()
	{
		if( inputLeft )
			return Edges.Left;
		else if( inputRight )
			return Edges.Right;
		else if( inputDown )
			return Edges.Below;
		else if( inputUp )
			return Edges.Above;
		else
			return Edges.None;
	}
	public static Vector2Int EdgeToDirection( Edges edge )
	{
		if( edge == Edges.Left )
			return Vector2Int.left;
		else if( edge == Edges.Right )
			return Vector2Int.right;
		else if( edge == Edges.Below )
			return Vector2Int.down;
		else if( edge == Edges.Above )
			return Vector2Int.up;
		else
			return new Vector2Int( -1, -1 );
	}
}
