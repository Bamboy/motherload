using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterMover : MonoBehaviour 
{
	public float simulationSpeed = 1f;
	public float gravity = -9.81f;
	public float moveSpeed = 2.5f;

	[Space]
	[ShowIf("InPlayMode")] public MovementData movement;
	[ShowIf("InPlayMode")][EnumToggleButtons] public Edges collisionSides;
	bool InPlayMode() { return Application.isPlaying; }

	private static float bloatSpacing = 0.01f;
	[HideInInspector] new public BoxCollider2D collider; 
	[HideInInspector] public List<Collider2D> ignoredColliders;

	/// Scaled Time.deltaTime
	private float _deltaTime;

	void Start()
	{
		movement.position = new Vector2( transform.position.x, transform.position.y );
		collider = GetComponent<BoxCollider2D>();
		 
		ignoredColliders = new List<Collider2D>();
		ignoredColliders.Add( collider );
	}

	void Update()
	{
		_deltaTime = Time.deltaTime * simulationSpeed;

		//Check edges we are in contact with. Check for changes

		MovementData newMovement = new MovementData( movement ); //Private


		Vector2 inputForce = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * moveSpeed * _deltaTime;
		newMovement.velocity += inputForce;

		collisionSides = GetSurroundings( newMovement.position );
		CollisionModify( collisionSides, ref newMovement );

		MovementData prediction = Verlet( newMovement, collisionSides.HasFlag( Edges.Below ) ? Vector2.zero : Acceleration() ); //Private

		//NOTE this implementation for collisions is NOT accurate with Verlet!! Need a better solution...

		float distance = Vector2.Distance(movement.position, prediction.position);
		Vector2 direction = VectorExtras.Direction(movement.position, prediction.position);
		List<RaycastHit2D> hits = FilterIgnored( ignoredColliders, 
			Physics2D.BoxCastAll( movement.position, collider.size, 0f, direction, distance ) );

		if( hits.Count > 0 ) //Did we hit something?
		{
			RaycastHit2D hit = hits[0];

			prediction.position = hit.centroid + (hit.normal * bloatSpacing);
		}

		movement = prediction;

		transform.position = new Vector3( movement.position.x, movement.position.y, 0f );

		collisionSides = GetSurroundings( transform.position );
		ExternalCall( collisionSides );
	}
		
	void CollisionModify( Edges edges, ref MovementData move )
	{
		if( edges.HasFlag( Edges.Below ) && move.velocity.y < 0f )
		{
			move.velocity.y = 0f;
		}
		else if( edges.HasFlag( Edges.Above ) && move.velocity.y > 0f )
		{
			move.velocity.y = 0f;
		}
			
		if( edges.HasFlag( Edges.Left ) && move.velocity.x < 0f )
		{
			move.velocity.x = 0f;
		}
		else if( edges.HasFlag( Edges.Right ) && move.velocity.x > 0f )
		{
			move.velocity.x = 0f;
		}
	}

	void ExternalCall( Edges contacts )
	{
		SendMessage("OnEdgeState", contacts, SendMessageOptions.DontRequireReceiver);
	}

	/*
	///Velocity Verlet implementation for moving
	void Verlet( ref Vector2 position, ref Vector2 velocity, ref Vector2 acceleration ) //https://youtu.be/hG9SzQxaCm8?t=21m31s
	{
		position += (velocity * _deltaTime) + ((acceleration * 0.5f) * _deltaTime * _deltaTime);
		Vector2 newAcceleration = Acceleration(acceleration);

		velocity += (0.5f * (acceleration + newAcceleration)) * _deltaTime;
		acceleration = newAcceleration;

		DebugExtension.DebugArrow(transform.position, velocity, Color.green);
		DebugExtension.DebugArrow(transform.position, acceleration, Color.cyan);

	} */
	///Velocity Verlet implementation for moving
	MovementData Verlet( MovementData lastMovement, Vector2 acceleration )
	{
		MovementData output;

		output.position = lastMovement.position + (lastMovement.velocity * _deltaTime) + ((lastMovement.acceleration * 0.5f) * _deltaTime * _deltaTime);

		Vector2 newAcceleration = acceleration;
		output.velocity = lastMovement.velocity + (0.5f * (lastMovement.acceleration + newAcceleration)) * _deltaTime;

		output.acceleration = newAcceleration;

		return output;
	}
		
	Vector2 Acceleration( Vector2 lastAcceleration = new Vector2() )
	{
		if( collisionSides.HasFlag( Edges.Below ) )
			return new Vector2( 0f, 0f );
		else
			return new Vector2( lastAcceleration.x, gravity );
	}
		
	/// Returns the edges where we are in contact with another collider.
	Edges GetSurroundings( Vector2 pos )
	{
		Edges output = new Edges();
		List<RaycastHit2D> hits;

		float dist = bloatSpacing;

		//Above
		Vector2 castDir = Vector2.up;
		hits = FilterIgnored( ignoredColliders, 
			Physics2D.BoxCastAll( pos, collider.size, 0f, castDir, dist ) );
		output = output.SetFlag<Edges>( Edges.Above, hits.Count > 0 );

		Bounds debugBounds = new Bounds(collider.bounds.center + (new Vector3(castDir.x, castDir.y) * dist), collider.bounds.size);
		DebugExtension.DebugBounds( debugBounds, hits.Count > 0 ? Color.red : Color.green );

		//Below
		castDir = Vector2.down;
		hits = FilterIgnored( ignoredColliders, 
			Physics2D.BoxCastAll( pos, collider.size, 0f, castDir, dist ) );
		output = output.SetFlag<Edges>( Edges.Below, hits.Count > 0 );

		debugBounds = new Bounds(collider.bounds.center + (new Vector3(castDir.x, castDir.y) * dist), collider.bounds.size);
		DebugExtension.DebugBounds( debugBounds, hits.Count > 0 ? Color.red : Color.green );

		//Right
		castDir = Vector2.right;
		hits = FilterIgnored( ignoredColliders, 
			Physics2D.BoxCastAll( pos, collider.size, 0f, castDir, dist ) );
		output = output.SetFlag<Edges>( Edges.Right, hits.Count > 0 );

		debugBounds = new Bounds(collider.bounds.center + (new Vector3(castDir.x, castDir.y) * dist), collider.bounds.size);
		DebugExtension.DebugBounds( debugBounds, hits.Count > 0 ? Color.red : Color.green );

		//Left
		castDir = Vector2.left;
		hits = FilterIgnored( ignoredColliders, 
			Physics2D.BoxCastAll( pos, collider.size, 0f, castDir, dist ) );
		output = output.SetFlag<Edges>( Edges.Left, hits.Count > 0 );

		debugBounds = new Bounds(collider.bounds.center + (new Vector3(castDir.x, castDir.y) * dist), collider.bounds.size);
		DebugExtension.DebugBounds( debugBounds, hits.Count > 0 ? Color.red : Color.green );

		//None
		output = output.SetFlag<Edges>( Edges.None, 
			output.HasFlag(Edges.Above) == false && output.HasFlag(Edges.Below) == false &&
			output.HasFlag(Edges.Right) == false && output.HasFlag(Edges.Left) == false );

		return output;
	}

	/// Filter the specified data, removing 'ignored' from the collection.
	private static List<RaycastHit2D> FilterIgnored( List<Collider2D> ignored, IEnumerable<RaycastHit2D> data )
	{
		if( ignored == null || ignored.Count == 0 )
			return new List<RaycastHit2D>( data );
		
		List<RaycastHit2D> output = new List<RaycastHit2D>();
		foreach (RaycastHit2D hit in data) 
		{
			bool keep = true;
			foreach (Collider2D col in ignored) 
			{
				if( col == null )
					continue;
				else if( col == hit.collider )
				{
					keep = false;
					break;
				}
			}
			if( keep )
				output.Add( hit );
		}
		return output;
	}


	/// Return what side of our box collider was impacted.
	private static Edges GetCollisionSide( RaycastHit2D hit )
	{
		if( hit.collider == null )
			return Edges.None;
		
		Vector2Int sign = VectorExtras.Sign01( -hit.normal );
		if( sign.y != 0 ) //Prioritize top or bottom collisions first
			return sign.y == 1 ? Edges.Above : Edges.Below;
		else if( sign.x != 0 )
			return sign.x == 1 ? Edges.Right : Edges.Left;
		else
			return Edges.None;
	}
}

