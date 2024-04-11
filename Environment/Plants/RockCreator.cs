using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockCreator : MonoBehaviour{
    public GameObject rockPrefab1;
    public GameObject rockPrefab2;
    public GameObject rockPrefab3;
    public GameObject rockPrefab4;
    public GameObject rockPrefab5;
    public GameObject rockPrefab6;
    public GameObject rockPrefab7;
    public GameObject rockPrefab8;
    public GameObject rockPrefab9;
    public GameObject rockPrefab10;
    public GameObject rockPrefab11;
    public GameObject rockPrefab12;
    public GameObject rockPrefab13;
    public GameObject rockPrefab14;
    public GameObject rockPrefab15;
    public GameObject rockPrefab16;
    public GameObject rockPrefab17;
    public GameObject rockPrefab18;
    public Material mat1;   
    public Material mat2;   
    public Material mat3;   
    public Material mat4;   
    public Material mat5;   
    public int maxRot = 4;
    public float scale = 0.2f;
    public float height = 0.5f;
    

    public string rockName = "Rock";

    public MapArray mapArray;
    public EnvironmentClass env;

    Cell[,] map;
    int mapSize;
    
    public void CreateRock(){
        //choose which rock to make
        int randRock = Random.Range(0,18);
        int randMat = Random.Range(0,5);
        Material mat;
        switch (randMat){
            case 0:
                mat = mat1;
                break;
            case 1:
                mat = mat2;
                break;
            case 2:
                mat = mat3;
                break;
            case 3:
                mat = mat4;
                break;
            default:
                mat = mat5;
                break;
            
        }
        GameObject rockPrefab = rockPrefab1;
        switch (randRock){
            case 0:
                rockPrefab = rockPrefab1;
                break;
            case 1:
                rockPrefab = rockPrefab2;
                break;
            case 2:
                rockPrefab = rockPrefab3;
                break;
            case 3:
                rockPrefab = rockPrefab4;
                break;
            case 4:
                rockPrefab = rockPrefab5;
                break;
            case 5:
                rockPrefab = rockPrefab6;
                break;
            case 6:
                rockPrefab = rockPrefab7;
                break;
            case 7:
                rockPrefab = rockPrefab8;
                break;
            case 8:
                rockPrefab = rockPrefab9;
                break;
            case 9:
                rockPrefab = rockPrefab10;
                break;
            case 10:
                rockPrefab = rockPrefab11;
                break;
            case 11:
                rockPrefab = rockPrefab12;
                break;
            case 12:
                rockPrefab = rockPrefab13;
                break;
            case 13:
                rockPrefab = rockPrefab14;
                break;
            case 14:
                rockPrefab = rockPrefab15;
                break;
            case 15:
                rockPrefab = rockPrefab16;
                break;
            case 16:
                rockPrefab = rockPrefab17;
                break;
            case 17:
                rockPrefab = rockPrefab18;
                break;
            default:
                rockPrefab = rockPrefab1;
                break;
        }
        float rotX = -90;
        float rotY = 0;
        float rotZ = 0;
        Quaternion rot = Quaternion.Euler (rotX, rotY, rotZ);
        rockPrefab = Instantiate(rockPrefab, new Vector3(0,0,0), rot);
        rockPrefab.GetComponent<Renderer>().material = mat;
        this.tag = rockName;
        rockPrefab.transform.parent = this.transform;
        map = mapArray.mapArray;
        mapSize = env.mapSize - 1;
        InitRock();
    }
    
    void InitRock(){
        int x = Random.Range(0, mapSize);
        int y = Random.Range(0, mapSize);
        
        while (map[x,y].whatsInside != "Empty"){
            x = Random.Range(0,mapSize);
            y = Random.Range(0,mapSize);
        }
        this.transform.position = new Vector3(x, height, y);
        this.transform.localScale = new Vector3(scale, scale, scale);
        
        map[x,y].whatsInside = rockName;
        map[x,y].Name = rockName;
    }
}
