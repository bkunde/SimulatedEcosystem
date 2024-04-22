using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : CreatureClass 
{
    public GameObject rabbitMalePrefab;
    public GameObject rabbitFemalePrefab;

	public void CreateStartRabbit(){
		mSpeed = Random.Range(2, 6);
		mHungerRate = 0.2f * mSpeed;
		mThirstRate = 0.34f * mSpeed;
        mReproductiveRate = (float)Random.Range(6, 10);
		mSightRange = Random.Range(2, 8);
        mDontMoveChance = Random.Range(25, 50);
		mCreatureName = "Rabbit";
		mGestationPeriod = ((float)Random.Range(5, 12))/10;
		mLifespan = Random.Range(4, 6);
        mDiet = foodSource.Berry;
        mAge = 1f;
        int r = Random.Range(0,2);
        if (r == 0){ 
            mSex = "Male";
            creature = rabbitMalePrefab;
        }else if (r == 1){
            mSex = "Female";
            creature = rabbitFemalePrefab;
        }
		CreateCreature(true);
        dc.CollectData(mCreatureName, "born");
	}
	
	public void CreateRabbit(int x, int y, Rabbit parent){
		mHungerRate = parent.mHungerRate;
		mThirstRate = parent.mThirstRate;
        mReproductiveRate = parent.mReproductiveRate;
		mSpeed = parent.mSpeed;
		mSightRange = parent.mSightRange;
        mDontMoveChance = parent.mDontMoveChance;
		mCreatureName = "Rabbit";
		mGestationPeriod = parent.mGestationPeriod;
		mLifespan = parent.mLifespan;
        mColor = Color.yellow;
        mDiet = foodSource.Berry;
        rowLoc = x;
        colLoc = y;
        int r = Random.Range(0,2);
        if (r == 0){ 
            mSex = "Male";
            creature = rabbitMalePrefab;
        }else if (r == 1){
            mSex = "Female";
            creature = rabbitFemalePrefab;
        }
		CreateCreature(false);
        dc.CollectData(mCreatureName, "born");
	}
	
}
