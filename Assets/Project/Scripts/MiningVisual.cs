using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MiningVisual : MonoBehaviour 
{
	public List<Sprite> animSprites;

	new private SpriteRenderer renderer;

	public ParticleSystem dirtSpawner;
	public float dirtRate = 8.5f;
	void Start () 
	{
		renderer = GetComponent<SpriteRenderer>();
		dirtSpawner = GetComponentInChildren<ParticleSystem>();
	}

	void Update()
	{
		if( visible )
		{
			Vector3 pos = GameManager.singleton.player.transform.position;
			dirtSpawner.transform.position = new Vector3( pos.x, pos.y, -5f );
		}
	}

	private bool _visible;
	public bool visible{
		get{ return _visible; }
		set
		{
			ParticleSystem.EmissionModule emitter = dirtSpawner.emission;
			if( value )
			{


			}
			else
			{

			}


			emitter.rateOverTimeMultiplier = value ? dirtRate : 0f;
			renderer.enabled = value;
			_visible = value;
		}
	}

	public void Set( float percentage )
	{
		//Mathf.FloorToInt( animSprites.Count * percentage );
		percentage = Mathf.Clamp01( percentage );
		int i = Mathf.FloorToInt( animSprites.Count * percentage );
		i = Mathf.Clamp( i, 0, animSprites.Count - 1 );

		Debug.Log( i + ", " + percentage );
		renderer.sprite = animSprites[ i ];
	}

	public void SetTile( Vector2Int pos )
	{
		transform.position = GameManager.singleton.level.transform.localPosition + new Vector3( pos.x, pos.y, 0f ) + new Vector3(0.5f, -0.5f, 0f);
	}

}
