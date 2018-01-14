using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(CharacterMover))]
public class Player : MonoBehaviour 
{
	static Player instance;
	public static Player singleton{ get{ return instance; } }

	[HideInInspector]public CharacterMover mover;


	/// What tile the player is inside of
	public Vector2Int tileCoord
	{
		get{ return GameManager.singleton.level.WorldToTile( transform.position ); }
	}

	public float miningTime = 1f;

	void Awake()
	{
		if( instance == null )
			instance = this;
		else
		{
			Destroy( this.gameObject );
			return;
		}

		mover = GetComponent<CharacterMover>();
	}


	public Vector2Int tilePos;
	void LateUpdate()
	{
		tilePos = tileCoord;
	}

	public MiningVisual minerVisual;


	private Edges _lastContacts;
	public void OnEdgeState( Edges contacts )
	{
		/*
		if( _lastContacts.HasFlag( Edges.Below ) == false && contacts.HasFlag( Edges.Below ) )
		{
			//We just hit the ground.
			mover.movement.velocity.x /= 20f;
		} */

		isOnGround = contacts.HasFlag( Edges.Below );



		if( isOnGround == false && mining == true )
			miningTile = new Vector2Int( -1, -1 );

		if( isOnGround && mining == false )
		{
			if( contacts.HasFlag( Edges.Left ) && Input.GetKeyDown( KeyCode.A ) )
			{
				miningTile = tileCoord + new Vector2Int(-1, 0);
				StartCoroutine( MineTimer() );
			}
			else if( contacts.HasFlag( Edges.Right ) && Input.GetKeyDown( KeyCode.D ) )
			{
				miningTile = tileCoord + new Vector2Int(1, 0);
				StartCoroutine( MineTimer() );
			}
		}
			
		_lastContacts = contacts;
	}

	public bool isOnGround{ get; private set; }

	public Vector2Int miningTile = new Vector2Int( -1, -1 );
	public bool mining{ get{ return miningTile != new Vector2Int( -1, -1 ); } }

	IEnumerator MineTimer()
	{
		float startTime = Time.time;
		float endime = Time.time + miningTime;
		minerVisual.visible = true;
		minerVisual.SetTile( miningTile );

		while( mining == true )
		{
			float progress = VectorExtras.ReverseLerp( Time.time, startTime, endime );

			minerVisual.Set( progress );
			if( Time.time >= endime )
			{
				GameManager.singleton.level.blocks[miningTile.x, miningTile.y] = 0;
				GameManager.singleton.level.UpdateMap();
				break;
			}

			yield return null;
		}
		minerVisual.visible = false;
		miningTile = new Vector2Int( -1, -1 );
	}
}
