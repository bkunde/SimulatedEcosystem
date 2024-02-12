using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureClass : MonoBehaviour
{
    //creature parameters
    public float mHungerRate = 0.1f;
    public float mThirstRate = 0.5f;

    public float mCurrentHunger = 0f;
    public float mCurrentThirst = 0f;
    
    //location
    public int rowLoc;		//what row the creature is in
    public int colLoc;		//what col the creature is in
				
    public enum behaviorState {Exploring, FindingFood, FindingWater};			
				

    public behaviorState mCurrentBehavior;

    void Start(){
	    mCurrentBehavior = behaviorState.Exploring;
	    //gameObject.AddComponent<Rigidbody>(); 
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
	    else{
		   mCurrentBehavior = behaviorState.Exploring;
	    }

	    //WhatBehaviorState is the creature in
	    if (mCurrentBehavior == behaviorState.Exploring){
		    Move();
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
    } 

    public void FindWater(){
	    Debug.Log("Finding Thirst");
    }

    public void Move(){
	    Debug.Log("Moving");
    }

    public void Die(){
	    Destroy(gameObject);
    }
}
