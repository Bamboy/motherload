using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MovementData
{
	public Vector2 position;
	public Vector2 velocity;
	public Vector2 acceleration;

	public MovementData( MovementData copyFrom )
	{
		this.position = copyFrom.position;
		this.velocity = copyFrom.velocity;
		this.acceleration = copyFrom.acceleration;
	}
	public MovementData( Vector2 pos, Vector2 vel )
	{
		this.position = pos;
		this.velocity = vel;
		this.acceleration = Vector2.zero;
	}
	public MovementData( Vector2 pos, Vector2 vel, Vector2 accel )
	{
		this.position = pos;
		this.velocity = vel;
		this.acceleration = accel;
	}
}