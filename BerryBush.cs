using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryBush : MonoBehaviour
{
    public bool isEaten;
    public GameObject bush;

    public EnvironmentClass EnvironmentClass;
 
    void Start()
    {

	bush = GameObject.CreatePrimitive(PrimitiveType.Sphere);	
	
	InitBush();


    }	
	
    public void InitBush(){
	int x = Random.Range(0,10);
	int y = Random.Range(0,10);

	bush.transform.position = new Vector3(x, 0.25f, y);
	bush.GetComponent<Renderer>().material.color = Color.red;

    }

}
