using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCreator : MonoBehaviour
{
    public GameObject treePrefab;
    public int maxRot = 4;
    public float scale = 0.2f;
    

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
        float height = Random.Range(1.5f, 2.2f);
        this.transform.position = new Vector3(x, height, y);
        this.transform.localScale = new Vector3(scale, scale, scale);
        
        map[x,y].whatsInside = treeName;
        map[x,y].Name = treeName;
    }
}
