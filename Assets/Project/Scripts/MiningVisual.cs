using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MiningVisual : MonoBehaviour 
{
	public List<Sprite> animSprites;

	new private SpriteRenderer renderer;


	void Start () 
	{
		renderer = GetComponent<SpriteRenderer>();
	}

	private bool _visible;
	public bool visible{
		get{ return _visible; }
		set
		{
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
