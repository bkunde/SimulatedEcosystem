using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCreator : MonoBehaviour
{
    public GameObject treePrefab1;
    public GameObject treePrefab2;
    public GameObject treePrefab3;
    public GameObject treePrefab4;
    public GameObject treePrefab5;
    public GameObject treePrefab6;
    public GameObject treePrefab7;
    public GameObject treePrefab8;
    public GameObject treePrefab9;
    public GameObject treePrefab10;
    public GameObject treePrefab11;
    public GameObject treePrefab12;
    public int maxRot = 4;
    public float scale = 1f;
    

    public string treeName = "Tree";

    public MapArray mapArray;
    public EnvironmentClass env;

    Cell[,] map;
    int mapSize;
    
    public void CreateTree(){
        float randRot = (float)(Random.Range(0,100) /100);
        float rotX = Mathf.Lerp (-maxRot, maxRot, (float) randRot);
        float rotZ = Mathf.Lerp (-maxRot, maxRot, (float) randRot);
        float rotY = Random.Range(0,360);
        Quaternion rot = Quaternion.Euler (rotX, rotY, rotZ);

        //choose tree prefab
        int randTree = Random.Range(0,11);
        GameObject treePrefab = treePrefab1;
        switch (randTree){
            case 0:
                treePrefab = treePrefab1;
                break;
            case 1:
                treePrefab = treePrefab2;
                break;
            case 2:
                treePrefab = treePrefab3;
                break;
            case 3:
                treePrefab = treePrefab4;
                break;
            case 4:
                treePrefab = treePrefab5;
                break;
            case 5:
                treePrefab = treePrefab6;
                break;
            case 6:
                treePrefab = treePrefab7;
                break;
            case 7:
                treePrefab = treePrefab8;
                break;
            case 8:
                treePrefab = treePrefab9;
                break;
            case 9:
                treePrefab = treePrefab10;
                break;
            case 10:
                treePrefab = treePrefab11;
                break;
            default:
                treePrefab = treePrefab12;
                break;
        }

        treePrefab = Instantiate(treePrefab, new Vector3(0,0,0), rot);
        this.tag = treeName;
        treePrefab.transform.parent = this.transform;
        map = mapArray.mapArray;
        mapSize = env.mapSize - 1;
        InitTree();
    }
    
    void InitTree(){
        int x = Random.Range(0, mapSize);
        int y = Random.Range(0, mapSize);
        
        while (map[x,y].whatsInside != "Empty"){
            x = Random.Range(0,mapSize);
            y = Random.Range(0,mapSize);
        }
        float height = 0f;
        this.transform.position = new Vector3(x, height, y);
        this.transform.localScale = new Vector3(scale, scale, scale);
        
        map[x,y].whatsInside = treeName;
        map[x,y].Name = treeName;
    }
}
