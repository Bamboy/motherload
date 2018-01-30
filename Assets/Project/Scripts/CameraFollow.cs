using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour 
{
	public Transform target;

	public float smoothTime = 1f;
	private Vector3 velocity;

	//TODO - add level bounds clamping

	void LateUpdate()
	{
		Vector3 movement = Vector3.SmoothDamp (transform.position, target.position, ref velocity, smoothTime);
		transform.position = new Vector3 (movement.x, movement.y, -10f);
	}

}
