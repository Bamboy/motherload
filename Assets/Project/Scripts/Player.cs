using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(CharacterMover))]
public class Player : MonoBehaviour 
{
	static Player instance;
	public static Player singleton{ get{ return instance; } } //TODO we probably don't want this incase we want multiple players in the future

	[HideInInspector]public CharacterMover mover;


	[ShowIf("InPlayMode")][EnumToggleButtons] public Edges a;
	[ShowIf("InPlayMode")][EnumToggleButtons] public Edges b;
	[ShowIf("InPlayMode")][EnumToggleButtons] public Edges c;
	bool InPlayMode() { return Application.isPlaying; }
	void Update()
	{
		a = GameManager.InputToEdge();
		b = _lastContacts;
		c = (a & b) & ~Edges.None; // & ~Edges.none removes None from calculation
	}


	/// What tile the player is inside of
	public Vector2Int tileCoord
	{
		get{ return GameManager.singleton.level.WorldToTile( transform.position ); }
	}

	public float miningStartDelay = 0.333f;
	public float miningTime = 1f;
	public float stopMiningVel = 2f;

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
	void Start()
	{
		StartCoroutine( MineStartDelay() );
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


		/*
		if( isOnGround == false && isMining == true )
			miningTile = new Vector2Int( -1, -1 );

		if( isOnGround && isMining == false )
		{
			if( GameManager.inputAny )
			{
				
			}


			if( contacts.HasFlag( Edges.Left ) && GameManager.inputLeft )
			{
				miningTile = tileCoord + new Vector2Int(-1, 0);

				StartCoroutine( MineTimer() );
			}
			else if( contacts.HasFlag( Edges.Right ) && GameManager.inputRight )
			{
				miningTile = tileCoord + new Vector2Int(1, 0);
				StartCoroutine( MineTimer() );
			}
		} */
			
		_lastContacts = contacts;
	}

	public bool isOnGround{ get; private set; }

	/// Which tile we're trying to mine
	public Vector2Int miningTile = new Vector2Int( -1, -1 );

	/// Are we currently trying to mine a tile?
	public bool isMining{ get{ return miningTile != new Vector2Int( -1, -1 ); } }

	/// Which direction we're mining in.
	public Edges miningDirection = Edges.None;
	

	IEnumerator MineStartDelay()
	{
		float targetTime = 0f;
		while( true )
		{
			miningDirection = (GameManager.InputToEdge() & _lastContacts) & ~Edges.None; // & ~Edges.none removes bit None from calculation

			if( isMining == true ) //Are we trying to start mining?
			{
				float startTime = Time.time; //'targetTime' is not used here since we would like to track progress of our timer
				float endime = Time.time + miningTime;
				minerVisual.visible = true;
				minerVisual.SetTile( miningTile );

				GameManager.inputEnabled = false;
				mover.isSimulating = false; //Disable player controller so we can move the player manually.

				Vector3 playerStart = transform.position;
				Vector3 playerEnd = new Vector3( miningTile.x + 0.5f, miningTile.y + 0.5f, 0f );

				while( true )
				{
					float progress = VectorExtras.ReverseLerp( Time.time, startTime, endime );

					transform.position = Vector3.Lerp( playerStart, playerEnd, progress );



					minerVisual.Set( progress );
					if( Time.time >= endime )
					{
						GameManager.singleton.level.blocks[miningTile.x, miningTile.y] = 0;
						GameManager.singleton.level.UpdateMap();
						transform.position = playerEnd;
						mover.movement.position = new Vector2(playerEnd.x, playerEnd.y);
						break;
					}

					yield return null;
				}
				GameManager.inputEnabled = true;
				mover.isSimulating = true;

				minerVisual.visible = false;
				miningTile = new Vector2Int( -1, -1 );

				yield return null;
			}
			else
			{
				if( (int)(miningDirection & ~Edges.None) != 0 && isOnGround && mover.movement.velocity.magnitude < stopMiningVel ) //Check for any direction OTHER than none
				{
					Edges startingMine = miningDirection;
					while( true )
					{
						miningDirection = (GameManager.InputToEdge() & _lastContacts) & ~Edges.None; // & ~Edges.none removes bit None from calculation

						if( miningDirection != startingMine || isOnGround == false || mover.movement.velocity.magnitude > stopMiningVel )  //Check if we should stop beginning to mine
						{
							targetTime = miningStartDelay;
							startingMine = Edges.None;
							miningTile = GameManager.EdgeToDirection( Edges.None ); //Set no tile
							break;
						}
						else
						{  //As long as the starting inputs are held, countdown our timer before actually starting to mine.
							targetTime -= Time.deltaTime;
							if( targetTime <= 0f )
							{
								targetTime = miningTime;
								miningTile = tileCoord + GameManager.EdgeToDirection( miningDirection ); //Set the adjacent tile to be mined.
								break;
							}
						}
						yield return null;
					}
				}
			}

			yield return null;
		}
	}


}
