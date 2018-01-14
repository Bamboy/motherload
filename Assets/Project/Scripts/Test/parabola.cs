using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parabola : MonoBehaviour 
{
	/// <summary>
	/// height time (Jump duration)
	/// </summary>
	public float th = 3f;
	/// <summary>
	/// height
	/// </summary>
	public float h;
	/// <summary>
	/// Gravity
	/// </summary>
	public float g = -9.81f;
	/// <summary>
	/// Starting velocity
	/// </summary>
	public float v0;

	void Start()
	{
		h = 0f;
		th = 0f;
	}
	void Update()
	{
		//Linear();
	}

	void Linear( float t )
	{
		v0 = -g * th;


		transform.position = new Vector3( 0, v0, 0 );
	}

//	void F( float t )
//	{
//		return (g * t) + v0;
//	}
		

	void GravSolve()
	{

	}

	public float xHeight;
	//public float 
}
