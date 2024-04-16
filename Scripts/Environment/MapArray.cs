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
		Cell defaultCell = new Cell("NameNotSet", null, "Empty", (-1, -1));
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
				Debug.Log(mapArray[x,y].cellLoc);
			}
		}
	}
}

public struct Cell{
	public string Name; 
	public GameObject terrianTile;
	public string whatsInside;
	public (int x, int y) cellLoc;
	
	public Cell(string name, GameObject tile, string contents, (int x, int y) loc){
		Name = name;
		terrianTile = tile;
		whatsInside = contents;
		cellLoc = loc;
	}
}
