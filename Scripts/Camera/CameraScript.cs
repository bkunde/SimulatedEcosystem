using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject camera;
    public EnvironmentClass map;
    public float rot = 10f;

    
    
    void Start(){
        float mid = (float)(map.mapSize/2);    
        float height = (float)(map.mapSize/3);
        float z = (float)(-map.mapSize/10);
        camera.transform.position = new Vector3(mid, height, z);
        camera.transform.Rotate(rot, 0, 0);
    }
}
