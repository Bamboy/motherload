using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parabola2 : MonoBehaviour 
{
	public float jumpHeight = 3f;
	public float timeToJumpHeight = 2f;

	public float _gravity;
	public Vector3 _velocity;
	public Vector3 stepMove;

	public float _jumpGravity;
	public float _jumpVelocity;
	void Start () 
	{
		_jumpGravity = -(2f * jumpHeight) / Mathf.Pow(timeToJumpHeight, 2f);
		_jumpVelocity = Mathf.Abs(_jumpGravity) * timeToJumpHeight;
	}
	

	public float xDiff;
	public float yDiff;

	void Update () 
	{
		yDiff = (_velocity.y + (_gravity * Time.deltaTime * 0.5f)) * Time.deltaTime;

		//stepMove = (_velocity + (Vector3.up * _gravity * Time.deltaTime * 0.5f)) * Time.deltaTime;

		stepMove = new Vector3( 0f, yDiff, 0f );

		Debug.DrawRay( transform.position, stepMove.normalized, Color.red );

		transform.Translate( stepMove );
		_velocity.y += _gravity * Time.deltaTime;

		if( Input.GetKeyDown( KeyCode.Space ) )
			_velocity.y = _jumpVelocity;
	}
}
