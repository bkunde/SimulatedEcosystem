using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArray : MonoBehaviour
{
	[SerializeField] EnvironmentClass env;
	private int mapSize; 
	public Cell[,] mapArray;
	
	
	void Awake(){
		mapSize = env.mapSize;
		Cell defaultCell = new Cell("NameNotSet", null, "Empty");
		mapArray = new Cell[mapSize, mapSize];
		
		for (int x = 0; x < mapSize; x++){
			for (int y = 0; y < mapSize; y++){
				mapArray[x,y] = defaultCell;
			}
		}
		//PrintArray();
	}
	public void PrintArray(){
		for (int x = 0; x < mapSize; x++){
			for (int y = 0; y < mapSize; y++){
				Debug.Log(mapArray[x,y].Name);
				Debug.Log(mapArray[x,y].whatsInside);
			}
		}
	}
}

public struct Cell{
	public string Name; 
	public GameObject terrianTile;
	public string whatsInside;
	
	public Cell(string name, GameObject tile, string contents){
		Name = name;
		terrianTile = tile;
		whatsInside = contents;
	}
}
