using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureClass : MonoBehaviour
{
    public GameObject creature;
    public BerryBush berryBush;
    public MapArray mapArray;

    BerryBush bush;
    Cell[,] map;

    //creature parameters
    public float mHungerRate = 0.1f;
    public float mThirstRate = 0.5f;

    public float mCurrentHunger = 0f;
    public float mCurrentThirst = 0f;

    public float speed = 0.1f;
    
    //location
    public int rowLoc;		//what row the creature is in
    public int colLoc;		//what col the creature is in
				
    public enum behaviorState {Exploring, FindingFood, FindingWater};			
				

    public behaviorState mCurrentBehavior;

    void Start(){
	    creature = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
	    creature.transform.parent = this.transform;
	    map = mapArray.mapArray;
	    bush = berryBush;
	    //rb = creature.AddComponent<Rigidbody>();
	    mCurrentBehavior = behaviorState.Exploring;


		int x = Random.Range(0,10);
		int y = Random.Range(0,10);

		while (map[x,y].whatsInside != "Empty"){
			x = Random.Range(0,10);
			y = Random.Range(0,10);
		}
	    //creature.transform.position = new Vector3(x, 0.5f, y);
	    this.transform.position = new Vector3(x, 0.5f, y);
	    this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
	    //creature.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

	    creature.GetComponent<Renderer>().material.color = Color.blue;
	
	    map[x,y].whatsInside = "Creature";
	    
    }

    void Update()
    {
	    UpdateHunger();
	    UpdateThirst();

	    if (mCurrentHunger >= 50f){
		mCurrentBehavior = behaviorState.FindingFood;
	    }
	    else if (mCurrentHunger >= 100f){
		Die();
	    }

	    if (mCurrentThirst >= 50f){
		mCurrentBehavior = behaviorState.FindingWater;
	    }
	    else if (mCurrentThirst >= 100f){
		Die();
	    }
	    //else{
	//	mCurrentBehavior = behaviorState.Exploring;
	  //  }

	    //WhatBehaviorState is the creature in
	    if (mCurrentBehavior == behaviorState.Exploring){
		Explore();
	    }else if (mCurrentBehavior == behaviorState.FindingFood){
		FindFood();
	    }else if (mCurrentBehavior == behaviorState.FindingWater){
		FindWater();
	    }
        
    }

    public void UpdateHunger(){
	mCurrentHunger += mHungerRate * Time.deltaTime;
    }

    public void UpdateThirst(){
	mCurrentThirst += mThirstRate * Time.deltaTime;
    }

    public void FindFood(){
	Debug.Log("Finding Food");
	Vector3 target = new Vector3(bush.transform.position.x, 0.5f, bush.transform.position.z);
	this.transform.position = Vector3.MoveTowards(creature.transform.position, target, speed);
	//AstarSearch();
	
    } 

    public void FindWater(){
	Debug.Log("Finding Thirst");
    }

    public void Explore(){
	Debug.Log("Moving");
    }

    public void Die(){
	Destroy(gameObject);
    }



}
