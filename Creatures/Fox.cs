using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : CreatureClass 
{
    public GameObject foxMalePrefab;
    public GameObject foxFemalePrefab;

	public void CreateStartFox(){
        mHungerRate = 2f;
        mThirstRate = 1.5f;
        mReproductiveRate = 2f;
        mSpeed = 3;
        mSightRange = 3;
        mDontMoveChance = 20;
        mCreatureName = "Fox";
        mGestationPeriod = 1f;
        mLifespan = 5;
        height = 0.3f;
        mDiet = foodSource.Rabbit;
        int r = Random.Range(0,2);
        if (r == 0){
            mSex = "Male";
            creature = foxMalePrefab;
        }else if (r == 1){ 
            mSex = "Female";
            creature = foxFemalePrefab;
        }
        CreateCreature(true);
	}
	
	public void CreateFox(int x, int y){
        mHungerRate = 2f;
        mThirstRate = 1.5f;
        mReproductiveRate = 2f;
        mSpeed = 3;
        mSightRange = 3;
        mDontMoveChance = 20;
        mCreatureName = "Fox";
        mGestationPeriod = 1f;
        mLifespan = 5;
        height = 0.3f;
        mDiet = foodSource.Rabbit;
        rowLoc = x;
        colLoc = y;
        int r = Random.Range(0,2);
        if (r == 0){
            mSex = "Male";
            creature = foxMalePrefab;
        }else if (r == 1){ 
            mSex = "Female";
            creature = foxFemalePrefab;
        }
		CreateCreature(false);
	}
}
