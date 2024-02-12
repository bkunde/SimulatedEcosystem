using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentClass : MonoBehaviour
{
    public int mapSize = 10;
    //Enviromental Parameters
    //TODO
    //BiomeType
    //FoodArray
    //CreatureArray

    //MapArray
    public MapArray mapArray;
    public GameObject tile;
    public TerrainType[] regions;

    Cell[,] map;

    void Start(){
	map = mapArray.mapArray;
	InitializeMap();
    }
    void InitializeMap(){
	
	for (int x = 0; x < mapSize; x++){
	    for (int y = 0; y < mapSize; y++){
		tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
		tile.transform.parent = this.transform;
		tile.transform.position = new Vector3(x, 0, y);
		tile.transform.localScale = new Vector3(0.1f, 1, 0.1f);
		tile.GetComponent<Renderer>().material.color = regions[0].color;
		map[x,y].Name = "Grass";
		map[x,y].terrianTile = tile;
	    }
	}
    }
}

[System.Serializable]
public struct TerrainType{
	public string name;
	public Color color;
}

