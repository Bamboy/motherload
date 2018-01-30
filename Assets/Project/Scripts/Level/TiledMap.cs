using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class TiledMap : MonoBehaviour 
{
	public Vector2 mapSize = new Vector2( 32, 72 );

	public List<Vector2Int> solidTiles = new List<Vector2Int>();

	private List<Vector3> newVertices = new List<Vector3>();
	private List<int> newTriangles = new List<int>();
	private List<Vector2> newUV = new List<Vector2>();

	private Mesh mesh;

	/// How many tiles are in our texture sheet, as a percentage.
	private float tUnit = 0.25f;

	public byte[,] blocks;
	private int squareCount;

	void Start () 
	{
		mesh = GetComponent<MeshFilter>().mesh;
		mesh.MarkDynamic();

		GenTerrain();
		BuildMesh();
		UpdateMesh();
	}

	[Button]
	public void UpdateMap()
	{
		BuildMesh();
		UpdateMesh();
	}

	public Vector2Int size{ get{ return new Vector2Int( blocks.GetLength(0), blocks.GetLength(1) ); } }

	int NoiseInt (int x, int y, float scale, float mag, float exp)
	{
		return (int) (Mathf.Pow ((Mathf.PerlinNoise(x/scale,y/scale)*mag),(exp) ));
	}

	void GenTerrain()
	{
		blocks = new byte[Mathf.FloorToInt(mapSize.x), Mathf.FloorToInt(mapSize.y)];

		for (int x = 0; x < blocks.GetLength(0); x++) 
		{
			for (int y = 0; y < blocks.GetLength(1); y++) 
			{
				//blocks[x,y] = (byte)Random.Range(0,3);
				blocks[x,y] = (byte)NoiseInt(x,y, 4f, 3f, 1.25f); 
			}
		}
		/*
		for(int px=0;px<blocks.GetLength(0);px++)
		{
			int stone = NoiseInt(px,0, 80,15,1);
			stone += NoiseInt(px,0, 50,30,1);
			stone += NoiseInt(px,0, 10,10,1);
			stone += 75;


			int dirt = NoiseInt(px,0, 100f,35,1);
			dirt += NoiseInt(px,100, 50,30,1);
			dirt += 75;


			for(int py=0;py<blocks.GetLength(1);py++)
			{
				if(py<stone)
				{
					blocks[px, py]=1;

					if(NoiseInt(px,py,12,16,1)>10){  //dirt spots
						blocks[px,py] = 2;

					}

					if(NoiseInt(px,py*2,16,14,1)>10){ //Caves
						blocks[px,py] = 0;

					}

				} 
				else if(py<dirt) 
				{
					blocks[px,py]=2;
				}
			}
		}*/
	}

	void BuildMesh()
	{
		foreach (Transform child in this.transform) 
		{
			Destroy( child.gameObject );
		}

		solidTiles = new List<Vector2Int>();
		for(int px = 0; px < blocks.GetLength(0); px++)
		{
			for(int py = 0; py < blocks.GetLength(1); py++)
			{
				TileData data; 
				TileLookup.tiles.TryGetValue( blocks[px,py], out data );
				if( data == null )
					data = TileLookup.tiles[0];

				if( TileShouldBeSolid(px,py) == true )
					solidTiles.Add( new Vector2Int(px,py) );


				GenSquare(px,py, data.texureCoord);

				/*
				if( blocks[px,py] != 0 )
				{
					if( TileShouldBeSolid(px,py) == true )
						solidTiles.Add( new Vector2Int(px,py) );

					if( blocks[px,py] == 1 )
						GenSquare(px,py,tStone);
					else if(blocks[px,py]==2)
						GenSquare(px,py,tGrass);
					
				} */
			}
		}

		foreach (Vector2Int tile in solidTiles) //Create colliders
		{
			GameObject obj = new GameObject("Tile Collider");
	
			BoxCollider2D col = obj.AddComponent<BoxCollider2D>();
			col.offset = new Vector2(0.5f, -0.5f);

			obj.transform.SetParent( this.transform );
			obj.transform.position = new Vector3( tile.x, tile.y, 0 ) + this.transform.position;
		}
	}

	byte Block (int x, int y)
	{
		if(x==-1 || x==blocks.GetLength(0) || y==-1 || y==blocks.GetLength(1))
			return (byte)1;

		return blocks[x,y];
	}


	bool TileShouldBeSolid(int x, int y)
	{
		if( blocks[x,y] != 0 )
		{
			if( x == 0 || x == size.x - 1 || y == 0 || y == size.y - 1 ) //Edges
				return true;

			if( Block(x,y+1) == 0 ||//Above
				Block(x,y-1) == 0 ||//Below
				Block(x-1,y) == 0 ||//Left
				Block(x+1,y) == 0 ) //Right
			{
				return true;
			}
		}
		return false;
	}
		

	void GenSquare(int x, int y, Vector2 texture)
	{
		newVertices.Add( new Vector3 (x  ,  y  , 0 ));
		newVertices.Add( new Vector3 (x + 1 ,  y  , 0 ));
		newVertices.Add( new Vector3 (x + 1 ,  y-1 , 0 ));
		newVertices.Add( new Vector3 (x  ,  y-1 , 0 ));

		newTriangles.Add(squareCount*4);
		newTriangles.Add((squareCount*4)+1);
		newTriangles.Add((squareCount*4)+3);
		newTriangles.Add((squareCount*4)+1);
		newTriangles.Add((squareCount*4)+2);
		newTriangles.Add((squareCount*4)+3);

		newUV.Add(new Vector2 (tUnit * texture.x, tUnit * texture.y + tUnit));
		newUV.Add(new Vector2 (tUnit * texture.x + tUnit, tUnit * texture.y + tUnit));
		newUV.Add(new Vector2 (tUnit * texture.x + tUnit, tUnit * texture.y));
		newUV.Add(new Vector2 (tUnit * texture.x, tUnit * texture.y));

		squareCount++;
	}

	void UpdateMesh () 
	{
		mesh.Clear ();
		mesh.vertices = newVertices.ToArray();
		mesh.triangles = newTriangles.ToArray();
		mesh.uv = newUV.ToArray();

		mesh.RecalculateNormals ();

		newVertices.Clear();
		newTriangles.Clear();
		newUV.Clear();
		squareCount = 0;
	}

	void OnDrawGizmos()
	{
		Bounds b = new Bounds( transform.position + Vector3.down, Vector3.zero ); // - Vector3.down
		b.Encapsulate( (transform.position + new Vector3( mapSize.x, mapSize.y, 0f ))  + Vector3.down) ;
			
		DebugExtension.DrawBounds( b, Color.blue );
	}

	public Vector2Int WorldToTile( Vector3 position )
	{
		Vector2 clamp = new Vector2( Mathf.Clamp( position.x, 0f, size.x ), Mathf.Clamp( position.y, 0f, size.y ) );
		return new Vector2Int( Mathf.FloorToInt( clamp.x ), Mathf.FloorToInt( clamp.y ) );
	}
	/*
	public Vector3 TileToWorld( Vector2Int position )
	{


		Vector2 clamp = new Vector2( Mathf.Clamp( position.x, 0f, size.x ), Mathf.Clamp( position.y, 0f, size.y ) );
		return new Vector2Int( Mathf.FloorToInt( clamp.x ), Mathf.FloorToInt( clamp.y ) );
	} */
}