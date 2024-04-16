using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astar;

public class CreatureClass : MonoBehaviour
{
	public GameObject creature;
    public BerryBush berryBush;
    public MapArray mapArray;
	public EnvironmentClass env;
    public DataCollection dc;

	int mapSize;
    BerryBush bush;
    Cell[,] map;

	string foodGoalName;

    //creature methods
    public float mHungerRate = 0.1f;
    public float mThirstRate = 0.5f;
	public float mReproductiveRate = 2.5f;

    public float mSpeed = 0.5f;
	public int mSightRange = 2;
    
    [Range(0,100)]
    public int mDontMoveChance = 30;

	public string mCreatureName = "Creature";
	public float mGestationPeriod = 2f;
	public int mLifespan = 10;
    public string mSex;

    public Color mColor;
    public foodSource mDiet;

    //parameters
    public float turnSpeed = 5f;
	public bool isDead = false;
    public bool reproduce = false;

	public float mAge = 0f;
    public float mCurrentHunger = 0f;
    public float mCurrentThirst = 0f;
	public float mReproductiveUrge = 0f;
	
    //location
    public int rowLoc;		//what row the creature is in
    public int colLoc;		//what col the creature is in
    public float height = 0f; 
	public (int x, int y) moveCoords;
	(int x, int y) previousBush;

	public bool Moving = false;
	bool lerping = false;
	bool resolvingActions = false;
	//public bool foundFood = false;

	List<int> actions = new List<int>();
				
    public enum behaviorState {Exploring, FindingFood, FindingWater, FindingMate, RunningAway};			
    public behaviorState mCurrentBehavior;

    public enum foodSource {Berry, Rabbit};

    public void CreateCreature(bool newCreature){
	    //creature = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        creature = Instantiate(creature, new Vector3(0,0,0), Quaternion.identity);
		creature.tag = mCreatureName;
		Rigidbody rb = gameObject.AddComponent<Rigidbody>();
		rb.isKinematic = true;
        rb.useGravity = true;
		this.transform.position = new Vector3(0,0,0);
	    creature.transform.parent = this.transform;
	    map = mapArray.mapArray;
		mapSize = env.mapSize;

	    mCurrentBehavior = behaviorState.Exploring;

        bush = berryBush;
        if (mDiet == foodSource.Berry){
            foodGoalName = berryBush.bushName;
        }
        else if (mDiet == foodSource.Rabbit){
            foodGoalName = "Rabbit";
        }

        if (newCreature){
            int x = UnityEngine.Random.Range(0,mapSize);
            int y = UnityEngine.Random.Range(0,mapSize);

            while (map[x,y].whatsInside != "Empty"){
                x = UnityEngine.Random.Range(0,mapSize);
                y = UnityEngine.Random.Range(0,mapSize);

            }
            rowLoc = x;
            colLoc = y;
        }
        
		this.transform.position = new Vector3(rowLoc, height, colLoc);
        this.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
	    //creature.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
	    //creature.GetComponent<Renderer>().material.color = mColor;
	
	    map[rowLoc,colLoc].whatsInside = mCreatureName;
    }

    public void UpdateCreature()
    {
		checkReproduce();
		checkHungerAndThirst();

		if ((rowLoc != previousBush.x) && (colLoc != previousBush.y))
			bush.isEaten = false;

		if ((map[rowLoc, colLoc].Name == "Water") && mCurrentBehavior == behaviorState.FindingWater)
			StartCoroutine(DrinkWater());

		if ((map[rowLoc, colLoc].whatsInside == foodGoalName) && mCurrentBehavior == behaviorState.FindingFood){
            if (mDiet == foodSource.Berry){
                foreach(BerryBush bush in env.bushes){
                    if ((bush.row == rowLoc) && (bush.col == colLoc) && (bush != null))
                        StartCoroutine(EatFood(bush));
                }
            }else if (mDiet == foodSource.Rabbit){
                foreach(Rabbit rabbit in env.rabbits){
                    if ((rabbit.rowLoc == rowLoc) && (rabbit.colLoc == colLoc) && (rabbit != null))
                        EatPrey(rabbit);
                }
            }
        }

        if (mCreatureName == "Rabbit"){
            int randChance = UnityEngine.Random.Range(0,100);
            if (mDontMoveChance > randChance){
                foreach(Fox fox in env.foxes){
                    int distance = Math.Abs(fox.rowLoc - rowLoc)+ Math.Abs(fox.colLoc - colLoc);
                    if (distance < 2)
                        mCurrentBehavior = behaviorState.RunningAway;
                }
            }
        }

    
		UpdateHunger();
		UpdateThirst();
		UpdateReproductive();
		UpdateAge();
		
		if((actions.Count > 0) && (!resolvingActions)){
			StartCoroutine(DoAction());
		}

		if (!Moving){
			if (mCurrentBehavior == behaviorState.Exploring){
                actions.Clear();
				StartCoroutine(Explore());
			}else if (mCurrentBehavior == behaviorState.FindingFood){
                if (mDiet == foodSource.Berry){
                    actions.Clear();
                    FindFood();
                }
                else if (mDiet == foodSource.Rabbit){
                    actions.Clear();
                    FindRabbit();
                }
			}else if (mCurrentBehavior == behaviorState.FindingWater){
                actions.Clear();
				FindWater();
			}else if (mCurrentBehavior == behaviorState.FindingMate){
                actions.Clear();
				FindMate();
			}else if (mCurrentBehavior == behaviorState.RunningAway){
                actions.Clear();
                RunAway();
            }
		}
    }
	
	void checkHungerAndThirst(){
	    if (mCurrentHunger >= 50f){
			mCurrentBehavior = behaviorState.FindingFood;
	    }
	    if (mCurrentHunger >= 100f){
			Die("starvation");
	    }

	    if (mCurrentThirst >= 50f){
			mCurrentBehavior = behaviorState.FindingWater;
	    }
	    if (mCurrentThirst >= 100f){
			Die("thirst");
	    }
		if ((mCurrentHunger < 50f && mCurrentThirst < 50f) && (mReproductiveUrge < 75))
			mCurrentBehavior = behaviorState.Exploring;
	}
	
	void checkReproduce(){
        if (mReproductiveUrge <= 10) reproduce = false;
		if (mReproductiveUrge >= 75f)
			mCurrentBehavior = behaviorState.FindingMate;
	}

    void UpdateHunger(){
		mCurrentHunger += mHungerRate * Time.deltaTime;
    }

    void UpdateThirst(){
		mCurrentThirst += mThirstRate * Time.deltaTime;
    }

    void UpdateReproductive(){
		if (mAge >= mGestationPeriod)
			mReproductiveUrge += mReproductiveRate * Time.deltaTime;
    }

    void UpdateAge(){
		mAge += 0.02f * Time.deltaTime;
		if ((int)(mAge/100) == 1)
			mAge += 1;
		if (mAge >= mLifespan)
			Die("age");
		else if (mAge >= mGestationPeriod){ 
            if (mCreatureName == "Fox"){
                height = 0.45f;
                this.transform.localScale = new Vector3(0.13f, 0.13f, 0.13f);
            }else
                this.transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
        }
    }

	IEnumerator DoAstar(string goal){
		AstarSearch astar = new AstarSearch(goal);
		//Start state
		(int x, int y) currentLoc = (rowLoc, colLoc);
		State s0 = new State(map, mSightRange, currentLoc);
		//Node = state, pnode, action, depth, f, g
		Node node = new Node(s0, null, 0, 0, 0, 0);
		node = astar.AstarSearchFunc(node);
		List<int> newActions = new List<int>();
		if (node.mPnode != null){
			previousBush = node.mState.CreatureLoc;
			while (node.mDepth > 0){
				newActions.Add(node.mAction);
				node = node.mPnode;
			}
			newActions.Reverse();
			//printActions(actions);
			foreach(int action in newActions){	
				actions.Add(action);
			}
		}else{
			StartCoroutine(Explore());
		}
		yield return null;
	}
		
	void GoToLoc(int newRow, int newCol){
		int rows = newRow - rowLoc;
		int cols = newCol - colLoc;	

        int xDist = newRow - rowLoc;
        int yDist = newCol - colLoc;

        if (xDist > 0)
            actions.Add(1);
        else if (xDist < 0)
            actions.Add(3);
        if (yDist > 0)
            actions.Add(0);
        else if (yDist < 0)
            actions.Add(2);
        
    }

    void FindFood(){
		//Debug.Log("Finding Food");
		StartCoroutine(DoAstar(foodGoalName));
    } 

    void FindWater(){
		//Debug.Log("Finding Thirst");
		string goal = "Water";
		StartCoroutine(DoAstar(goal));
    }

	void FindMate(){
        if (mCreatureName == "Rabbit"){
            Rabbit closestRabbit = null;
            int distance;
            int closest = int.MaxValue;
            foreach(Rabbit rabbit in env.rabbits){
                if (!(rabbit == this) && (rabbit != null)){
                    if (isClose(rabbit)){
                        if (rabbit.mAge >= mGestationPeriod && (rabbit.mSex != this.mSex) && (rabbit.mReproductiveUrge >= 75)){
                            distance = Math.Abs(rabbit.rowLoc - rowLoc)+ Math.Abs(rabbit.colLoc - colLoc);
                            if (distance < closest){
                                closestRabbit = rabbit;
                            }
                        }else
                            StartCoroutine(Explore());
                    }else{
                        StartCoroutine(Explore());
                    }
                    if (closestRabbit != null)
                        StartCoroutine(Chase(closestRabbit));

                    if (env.rabbits.Count == 0){
                        StartCoroutine(Explore());
                    }
                }
            }
        }
        else if (mCreatureName == "Fox"){
            Fox closestFox = null;
            int distance;
            int closest = int.MaxValue;
            foreach(Fox fox in env.foxes){
                if (!(fox == this) && (fox != null)){
                    if (isClose(fox)){
                        if (fox.mAge >= mGestationPeriod && (fox.mSex != this.mSex) && (fox.mReproductiveUrge >= 75)){
                            distance = Math.Abs(fox.rowLoc - rowLoc)+ Math.Abs(fox.colLoc - colLoc);
                            if (distance < closest){
                                closestFox = fox;
                            }
                        }else
                            StartCoroutine(Explore());
                    }else{
                        StartCoroutine(Explore());
                    }
                    if (closestFox != null)
                        StartCoroutine(Chase(closestFox));

                    if (env.foxes.Count == 0){
                        StartCoroutine(Explore());
                    }
                }
            }
        }
    }

    IEnumerator Chase(CreatureClass mate){
        if ((mate.rowLoc == rowLoc) && (mate.colLoc == colLoc)){
            Reproduce(mate);
        }
        else{
            int distance = Math.Abs(mate.rowLoc - rowLoc)+ Math.Abs(mate.colLoc - colLoc);
            if ((distance <= 2) && (mSex == "Female"))
                DontMove();
            else
                GoToLoc(mate.rowLoc, mate.colLoc);
        }
        yield return null;
    }

    IEnumerator Hunt(CreatureClass prey){
        if ((prey.rowLoc == rowLoc) && (prey.colLoc == colLoc)){
            EatPrey(prey);
        }
        else
            GoToLoc(prey.rowLoc, prey.colLoc);

        yield return null;
    }

    void RunAway(){
        foreach(Fox predator in env.foxes){
            int rowDist = Math.Abs(rowLoc - predator.rowLoc);    
            int colDist = Math.Abs(colLoc - predator.colLoc);    
            
            if (rowDist < colDist){
                int rand = UnityEngine.Random.Range(0,1);
                if (rand == 0)
                    actions.Add(1);
                else
                    actions.Add(3);
            }
            else{
                int rand = UnityEngine.Random.Range(0,1);
                if (rand == 0)
                    actions.Add(0);
                else
                    actions.Add(2);
            }
        }
    }

	void FindRabbit(){
		//Debug.Log("FindingRabbit");
        Rabbit closestRabbit = null;
        int distance;
        int closest = int.MaxValue;
		foreach(Rabbit rabbit in env.rabbits){
            if (isClose(rabbit)){
                distance = Math.Abs(rabbit.rowLoc - rowLoc)+ Math.Abs(rabbit.colLoc - colLoc);
                if (distance < closest){
                    closestRabbit = rabbit;
                }
            }
            else{
                StartCoroutine(Explore());
            }
        }
        if (closestRabbit != null)
            StartCoroutine(Hunt(closestRabbit));

        if (env.rabbits.Count == 0){
            StartCoroutine(Explore());
        }
	}

    void EatPrey(CreatureClass prey){
        prey.Die("eaten");
        mCurrentHunger -= 75;
        if (mCurrentHunger < 0) mCurrentHunger = 0;
    }

    void GoTowardsLoc(int x, int y){
		int xDist = x - rowLoc;
		int yDist = y - colLoc;

		if (xDist > 0)
			//Move(1);
			actions.Add(1);
		else if (xDist < 0)
			//Move(3);
			actions.Add(3);
		if (yDist > 0)
			//Move(0);
			actions.Add(0);
		else if (yDist < 0)
			//Move(2);
			actions.Add(2);
    }
	
	bool isClose(CreatureClass creature){
		if ((Math.Abs(rowLoc - creature.rowLoc) <= mSightRange) && 
			(Math.Abs(colLoc - creature.colLoc) <= mSightRange)){
				return true;
			}
		return false;
	}

	int GetNewX(){
		int currentX = (int)this.transform.position.x;
		int x = UnityEngine.Random.Range(-mSightRange, mSightRange);
		int newRowLoc = currentX + x; 
		while (newRowLoc >= mapSize || newRowLoc < 0){
			x = UnityEngine.Random.Range(-mSightRange, mSightRange);
			newRowLoc = currentX + x; 
		}
		return newRowLoc;
	}	
	int GetNewY(){
		int currentY = (int)this.transform.position.z;
		int y = UnityEngine.Random.Range(-mSightRange, mSightRange);
		int newColLoc = currentY + y;
		while (newColLoc >= mapSize || newColLoc < 0){
			y = UnityEngine.Random.Range(-mSightRange, mSightRange);
			newColLoc = currentY + y;
		}
		return newColLoc;
	}
    IEnumerator Explore(){
		int dontMove= UnityEngine.Random.Range(0,100);
        if (dontMove < mDontMoveChance){
            actions.Add(4);
        }else{
            int newRowLoc = -1; 
            int newColLoc = -1;
            newRowLoc = GetNewX();
            newColLoc = GetNewY();
            
            while (map[newRowLoc, newColLoc].whatsInside != "Empty"){
                newRowLoc = GetNewX();
                newColLoc = GetNewY();
            }

            int xDist = newRowLoc - rowLoc;
            int yDist = newColLoc - colLoc;

            if (xDist > 0)
                //Move(1);
                actions.Add(1);
            else if (xDist < 0)
                //Move(3);
                actions.Add(3);
            if (yDist > 0)
                //Move(0);
                actions.Add(0);
            else if (yDist < 0)
                //Move(2);
                actions.Add(2);

        }
        yield return null;
    }

    public void Die(string causeOfDeath){
		if (map[rowLoc, colLoc].Name == "Water"){
			map[rowLoc, colLoc].whatsInside = "Water";
		}
		else if (map[rowLoc, colLoc].Name == berryBush.bushName){
			map[rowLoc, colLoc].whatsInside = berryBush.bushName;
		}else if (map[rowLoc, colLoc].Name == "Tree"){ 
			map[rowLoc, colLoc].whatsInside = "Tree";
		}else if (map[rowLoc, colLoc].Name == "Rock"){
			map[rowLoc, colLoc].whatsInside = "Rock";
		}else{
			map[rowLoc, colLoc].whatsInside = "Empty";
		}

		isDead = true;
        dc.CollectData(mCreatureName, causeOfDeath);
        
		Destroy(creature);
		Destroy(this);
    }
	
	IEnumerator DoAction(){
		resolvingActions = true;
		while (actions.Count > 0){
			yield return Move(actions[0]);
			actions.RemoveAt(0);
		}
		resolvingActions = false;
	}
    void LookAt(Vector3 target){
        Vector3 start = this.transform.position;
        if (start != target){
            Quaternion lookRot = Quaternion.LookRotation((target - start).normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnSpeed);
        }
    }

	IEnumerator LerpPosition(Vector3 target){
		lerping = true;
		float time = 0;
		float duration = 2;
		Vector3 start = this.transform.position;
		while (time < duration){
            LookAt(target);
			this.transform.position = Vector3.Lerp(start, target, (time/duration));
			time += mSpeed * Time.deltaTime;
			yield return null;
		}
		this.transform.position = target;

		if (map[rowLoc, colLoc].Name == "Water"){
			map[rowLoc, colLoc].whatsInside = "Water";
		}
		else if (map[rowLoc, colLoc].Name == berryBush.bushName){
			map[rowLoc, colLoc].whatsInside = berryBush.bushName;
		}else if (map[rowLoc, colLoc].Name == "Tree"){ 
			map[rowLoc, colLoc].whatsInside = "Tree";
		}else if (map[rowLoc, colLoc].Name == "Rock"){
			map[rowLoc, colLoc].whatsInside = "Rock";
		}else{
			map[rowLoc, colLoc].whatsInside = "Empty";
		}

		rowLoc = (int)Math.Round(this.transform.position.x);
		colLoc = (int)Math.Round(this.transform.position.z);
		//Debug.Log($"Creature Pos {rowLoc}, {colLoc}");
		map[rowLoc, colLoc].whatsInside = mCreatureName; 
		lerping = false;
		//Debug.Log("Finsihed Lerping");
		Moving = false;
	}

	IEnumerator Move(int dir){
		//Debug.Log($"Moving: {Moving}");
		Vector3 target = new Vector3(-1,-1,-1);
		if (!Moving){
			Moving = true;
			switch (dir){
			case 0: //up
				target = new Vector3(rowLoc, height, (int)colLoc+1);
				break;
			case 1: //right
				target = new Vector3((int)rowLoc+1, height, colLoc);
				break;
			case 2: //down
				target = new Vector3(rowLoc, height, (int)colLoc-1);
				break;
			case 3: //left
				target = new Vector3((int)rowLoc-1, height, colLoc);
				break;
            case 4: //dont move
                target = new Vector3(rowLoc, height, colLoc);
                break;
			default:
				Debug.LogWarning("Unknown action: " + dir);
				yield break;
			}
		}
        //check to stay in bounds
        if (target.x < 0)
            target.x = 0;
        else if (target.x >= mapSize)
            target.x = mapSize - 1;
        if (target.z < 0)
            target.z = 0;
        else if (target.z >= mapSize)
            target.z = mapSize - 1;
		if (!lerping){
			yield return LerpPosition(target);
		}
        lerping = false;
	}
	
	
	void Reproduce(CreatureClass creature){
        //Debug.Log("Reproducing");
        creature.mReproductiveUrge = 0;
		mReproductiveUrge = 0;
        reproduce = true;
	}
	
	
	IEnumerator EatFood(BerryBush bush){
        //Debug.Log($"Eating {bush}");
		if (bush != null){
			bush.EatBerries();
			mCurrentHunger -= 25;
			if (mCurrentHunger < 0) mCurrentHunger = 0;
		}
        yield return new WaitForSeconds(2);
	}
	
	IEnumerator DrinkWater(){
		mCurrentThirst -= 50;
		if (mCurrentThirst < 0) mCurrentThirst = 0;
        yield return new WaitForSeconds(2);
	}
    
    public void DontMove(){
        actions.Add(4);
    }
	
}
