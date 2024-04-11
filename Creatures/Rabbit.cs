using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : CreatureClass 
{
    public GameObject rabbitMalePrefab;
    public GameObject rabbitFemalePrefab;

	public void CreateStartRabbit(){
		mHungerRate = 1f;
		mThirstRate = 1.5f;
        mReproductiveRate = 5f;
		mSpeed = Random.Range(1, 6);
		mSightRange = Random.Range(2, 8);
        mDontMoveChance = 40;
		mCreatureName = "Rabbit";
		mGestationPeriod = ((float)Random.Range(5, 12))/10;
		mLifespan = Random.Range(2, 4);
        mDiet = foodSource.Berry;
        int r = Random.Range(0,2);
        if (r == 0){ 
            mSex = "Male";
            creature = rabbitMalePrefab;
        }else if (r == 1){
            mSex = "Female";
            creature = rabbitFemalePrefab;
        }
		CreateCreature(true);
	}
	
	public void CreateRabbit(int x, int y){
		mHungerRate = 1f;
		mThirstRate = 1.5f;
        mReproductiveRate = 5f;
		mSpeed = 5;
		mSightRange = 4;
        mDontMoveChance = 40;
		mCreatureName = "Rabbit";
		mGestationPeriod = 0.5f;
		mLifespan = 2;
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
	}
	
}
