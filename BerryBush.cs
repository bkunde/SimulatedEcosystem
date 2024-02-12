using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryBush : MonoBehaviour
{
    public bool isEaten;
    public GameObject bush;

    public MapArray mapArray;

    Cell[,] map;
 
    void Start()
    {
	bush = GameObject.CreatePrimitive(PrimitiveType.Sphere);	
	bush.transform.parent = this.transform;
	map = mapArray.mapArray;
	InitBush();

    }	
	
    public void InitBush(){
	int x = Random.Range(0,10);
	int y = Random.Range(0,10);

	while (map[x,y].whatsInside != "Empty"){
		x = Random.Range(0,10);
		y = Random.Range(0,10);
	}

	this.transform.position = new Vector3(x, 0.25f, y);
	bush.GetComponent<Renderer>().material.color = Color.red;
	
	map[x,y].whatsInside = "BerryBush";

    }

}
