using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : CreatureClass 
{
    public GameObject foxMalePrefab;
    public GameObject foxFemalePrefab;

	public void CreateStartFox(){
        mSpeed = Random.Range(2, 8);
        mHungerRate = 0.4f * mSpeed;
        mThirstRate = 0.5f * mSpeed;
        mReproductiveRate = (float)Random.Range(2, 4);
        mSightRange = Random.Range(2, 6);
        mDontMoveChance = Random.Range(15, 30);
        mCreatureName = "Fox";
        mGestationPeriod = ((float)Random.Range(10, 17))/10;
        mLifespan = Random.Range(3, 8);
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
        dc.CollectData(mCreatureName, "born");
	}
	
	public void CreateFox(int x, int y, Fox parent){
        mHungerRate = parent.mHungerRate;
        mThirstRate = parent.mThirstRate;
        mReproductiveRate = parent.mReproductiveRate;
        mSpeed = parent.mSpeed;
        mSightRange = parent.mSightRange;
        mDontMoveChance = parent.mDontMoveChance;
        mCreatureName = "Fox";
        mGestationPeriod = parent.mGestationPeriod;
        mLifespan = parent.mLifespan;
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
        dc.CollectData(mCreatureName, "born");
	}
}
