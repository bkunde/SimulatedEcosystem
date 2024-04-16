using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryBush : MonoBehaviour
{
    public bool isEaten = false;
    public GameObject bushPrefab1;
    public GameObject bushPrefab2;
    public GameObject bushPrefab3;
    public GameObject bush;
	SphereCollider sc;
    public float scale = 1f;
    public int maxRot = 4;
    public float height = 0;
	public int berries = 3;
	public float decreaseSize; 
	public string bushName = "BerryBush";
    public int row;
    public int col;

    public MapArray mapArray;
	public EnvironmentClass env;

    Cell[,] map;
	int mapSize;
 
	public void CreateBush(){
        int randBush = Random.Range(0,3);
        GameObject bushPrefab;
        switch (randBush){
            case 0:
                bushPrefab = bushPrefab1;
                break;
            case 1:
                bushPrefab = bushPrefab2;
                break;
            case 2:
                bushPrefab = bushPrefab3;
                break;
            default:
                bushPrefab = bushPrefab1;
                break;
        }

        float randRot = (float)(Random.Range(0,100) /100);
        float rotX = Mathf.Lerp (-maxRot, maxRot, (float) randRot);
        float rotZ = Mathf.Lerp (-maxRot, maxRot, (float) randRot);
        float rotY = Random.Range(0,360);
        Quaternion rot = Quaternion.Euler (rotX, rotY, rotZ);
        bush = Instantiate(bushPrefab, new Vector3(0,0,0), rot);
		bush.tag = bushName;
        this.tag = bushName;
        berries = Random.Range(3, 5);
		SphereCollider sc = bush.AddComponent<SphereCollider>();
		sc.isTrigger = true;
		bush.transform.parent = this.transform;
		map = mapArray.mapArray;
		mapSize = env.mapSize - 1;
		decreaseSize = (float)1/berries;
		InitBush();
	}
	
    void InitBush(){
		int x = Random.Range(0,mapSize);
		int y = Random.Range(0,mapSize);

		while (map[x,y].whatsInside != "Empty"){
			x = Random.Range(0,mapSize);
			y = Random.Range(0,mapSize);
		}

		this.transform.position = new Vector3(x, height, y);
		//bush.GetComponent<Renderer>().material.color = Color.red;
		
        row = x;
        col = y;
		map[x,y].whatsInside = bushName;
		map[x,y].Name = bushName;

	}

	public void EatBerries(){
		if (berries <= 1)
			DestoryBush();
		berries--;
		this.transform.localScale -= new Vector3(decreaseSize, decreaseSize, decreaseSize); 
        height += 0.1f;
		this.transform.position = new Vector3(row, height, col);
	}
	public void DestoryBush(){
        map[row, col].Name = "Grass";
        map[row, col].whatsInside = "BerryBush";
		Destroy(bush);
		Destroy(this);
	}
}
