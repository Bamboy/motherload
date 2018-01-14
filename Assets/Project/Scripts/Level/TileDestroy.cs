using UnityEngine;
using System.Collections;

public class TileDestroy : MonoBehaviour 
{

	public GameObject terrain;
	private TiledMap tScript;
	public GameObject target;
	private LayerMask layerMask = (1 << 0);


	void Start () {

		tScript = terrain.GetComponent<TiledMap>();

	}


	void Update () 
	{





		//float distance=Vector3.Distance(transform.position,target.transform.position);

		Vector2 start = new Vector2( transform.position.x, transform.position.y );
		Vector2 end = new Vector2( target.transform.position.x, target.transform.position.y );

		RaycastHit2D hit = Physics2D.Linecast(start, end);

		if( hit )
		{

			Debug.DrawLine(transform.position,hit.point,Color.red);

			Vector2 point = new Vector2(hit.point.x, hit.point.y);
			point += new Vector2(hit.normal.x,hit.normal.y) * 0.5f;

			Debug.DrawLine(hit.point,new Vector3(point.x,point.y, 0f),Color.magenta);



			Vector2Int hitTile = new Vector2Int(Mathf.Clamp( Mathf.RoundToInt(point.x - 0.5f), 0, tScript.size.x - 1),
				Mathf.Clamp( Mathf.RoundToInt(point.y + 0.5f), 0, tScript.size.y - 1));


			hitTile.x = Mathf.RoundToInt(hit.transform.position.x);
			hitTile.y = Mathf.RoundToInt(hit.transform.position.y);

			//tScript.blocks[Mathf.RoundToInt(point.x-.5f),Mathf.RoundToInt(point.y+.5f)] = 0;

			tScript.blocks[hitTile.x, hitTile.y] = 0;




			//tScript.update=true;
			tScript.UpdateMap();
		} 
		else 
		{
			Debug.DrawLine(transform.position,target.transform.position,Color.blue);
		}
	}
}