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
    public Cell[,] mMapArray {get; set;}
    public GameObject tile;
    public TerrainType[] regions;


    void Start(){
	InitializeMap();
    }
    void InitializeMap(){
	mMapArray = new Cell[mapSize, mapSize];
	
	for (int x = 0; x < mapSize; x++){
	    for (int y = 0; y < mapSize; y++){
		tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
		tile.transform.localScale = new Vector3(0.1f, 1, 0.1f);
		tile.transform.position = new Vector3(x, 0, y);
		tile.GetComponent<Renderer>().material.color = regions[0].color;
		mMapArray[x,y].name = "Grass";
		mMapArray[x,y].terrianTile = tile;
		mMapArray[x,y].whatsInside = "Empty";
	    }
	}
	Debug.Log(mMapArray);
    }
}

public struct Cell{
	public string name;
	public GameObject terrianTile;
	public string whatsInside;
}

/*
public class Cell{
	private GameObject mTerrianTile;
	public string mWhatsInside;

	public Cell(GameObject terrainTile, string contents){
		this.mWhatsInside = contents;
		this.mTerrianTile = terrainTile;
	}
	
	public string GetContents(){
		return mWhatsInside;
	}
	public void UpdateCell(string contents){
		mWhatsInside = contents;
	}
}
*/
[System.Serializable]
public struct TerrainType{
	public string name;
	public Color color;
}

