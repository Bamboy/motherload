using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnumTest : MonoBehaviour 
{
	[ShowIf("InPlayMode")][EnumToggleButtons] public Edges collisionSides;
	bool InPlayMode(){ return Application.isPlaying; }


	void Update () 
	{
		if( Input.GetKeyDown(KeyCode.Space) )
		{
			collisionSides = Edges.None;
			return;
		}

		if( Input.GetKeyDown(KeyCode.UpArrow) )
		{
			collisionSides = collisionSides.AddFlag<Edges>( Edges.Above );
			return;
		}

		if( Input.GetKeyDown(KeyCode.DownArrow) )
		{
			collisionSides = collisionSides.AddFlag<Edges>( Edges.Below );
			return;
		}
		if( Input.GetKeyDown(KeyCode.LeftArrow) )
		{
			collisionSides = collisionSides | Edges.Left;
			return;
		}

	}
}
