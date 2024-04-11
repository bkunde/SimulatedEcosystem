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

	public List<Node> reached = new List<Node>();
	List<int> actions = new List<int>();
				
    public enum behaviorState {Exploring, FindingFood, FindingWater, FindingMate};			
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
	
	    map[rowLoc,colLoc].whatsInside = "Creature";
	    
    }

    public void UpdateCreature()
    {
		checkReproduce();
		checkHungerAndThirst();

		if ((rowLoc != previousBush.x) && (colLoc != previousBush.y))
			bush.isEaten = false;

		if ((map[rowLoc, colLoc].Name == "Water") && mCurrentBehavior == behaviorState.FindingWater)
			DrinkWater();

		UpdateHunger();
		UpdateThirst();
		UpdateReproductive();
		UpdateAge();
		
		if((actions.Count > 0) && (!resolvingActions)){
			StartCoroutine(DoAction());
		}

		if (!Moving){
			if (mCurrentBehavior == behaviorState.Exploring){
				StartCoroutine(Explore());
			}else if (mCurrentBehavior == behaviorState.FindingFood){
                if (mDiet == foodSource.Berry){
                    FindFood();
                }
                else if (mDiet == foodSource.Rabbit){
                    FindRabbit();
                }
			}else if (mCurrentBehavior == behaviorState.FindingWater){
				FindWater();
			}else if (mCurrentBehavior == behaviorState.FindingMate){
				FindMate();
			}
		}
    }
	
	void checkHungerAndThirst(){
	    if (mCurrentHunger >= 50f){
			mCurrentBehavior = behaviorState.FindingFood;
	    }
	    if (mCurrentHunger >= 100f){
			Die();
	    }

	    if (mCurrentThirst >= 50f){
			mCurrentBehavior = behaviorState.FindingWater;
	    }
	    if (mCurrentThirst >= 100f){
			Die();
	    }
		if ((mCurrentHunger < 50f && mCurrentThirst < 50f) && (mCurrentBehavior != behaviorState.FindingMate))
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
			Die();
		else if (mAge >= mGestationPeriod){ 
            if (mCreatureName == "Fox"){
                height = 0.45f;
                this.transform.localScale = new Vector3(0.13f, 0.13f, 0.13f);
            }else
                this.transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
        }
    }

	IEnumerator DoAstar(string goal){
		AstarSearch astar = new AstarSearch(goal, reached);
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
			Debug.Log($"Couldn't find goal, starting explore");
			StartCoroutine(Explore());
		}
		yield return null;
	}
		
	void goToLoc(int newRow, int newCol){
		int rows = newRow - rowLoc;
		int cols = newCol - colLoc;	
		while (rows != 0){
			if (rows > 0){
				actions.Add(1);
				rows--;
			}else{
				actions.Add(3);
				rows++;
			}
		}
		while (cols != 0){
			if (cols > 0){
				actions.Add(0);
				cols--;
			}else{
				actions.Add(2);
				cols++;
			}
		}
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
		Debug.Log("FindingMate");
		foreach(Rabbit rabbit in env.rabbits){
			//check mate suitablity 
			if (!(rabbit == this)){
				if (rabbit.mAge >= mGestationPeriod && isClose(rabbit)){
                    int distance = Math.Abs(rabbit.rowLoc - rowLoc)+ Math.Abs(rabbit.colLoc - colLoc);
                    if (distance == 0){
                        Reproduce();
                    }
                    else if (distance <= 2){
                        if((rabbit.mSex == "Female")||(this.mSex == "Female"))
                            rabbit.DontMove();
                        else 
                            GoTowardsLoc(rabbit.rowLoc, rabbit.colLoc);
                    }
                    GoTowardsLoc(rabbit.rowLoc, rabbit.colLoc);
				}
				else{
					Debug.Log("No suitable mate found, exploring");
					StartCoroutine(Explore());
				}
			}
		}
	}



	void FindRabbit(){
		Debug.Log("FindingRabbit");
		foreach(Rabbit rabbit in env.rabbits){
            if (isClose(rabbit)){
                Debug.Log("Rabbit is close");
                int distance = Math.Abs(rabbit.rowLoc - rowLoc)+ Math.Abs(rabbit.colLoc - colLoc);
                if (distance == 0){
                    EatRabbit(rabbit);
                }
                GoTowardsLoc(rabbit.rowLoc, rabbit.colLoc);
            }
            else{
                Debug.Log("No suitable prey found, exploring");
                StartCoroutine(Explore());
            }
		}
	}

    void EatRabbit(Rabbit rabbit){
        rabbit.Die();
        mCurrentHunger -= 50;
        if (mCurrentHunger < 0) mCurrentHunger = 0;
    }

    void GoTowardsLoc(int x, int y){
        Debug.Log($"Moving towards {x}{x}");
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
	
	bool isClose(Rabbit rabbit){
		if ((Math.Abs(rowLoc - rabbit.rowLoc) <= 5) && 
			(Math.Abs(colLoc - rabbit.colLoc) <= 5)){
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
		//Debug.Log("Exploring");
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

    public void Die(){
		isDead = true;
        
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
		//Debug.Log("Lerping");
		//Debug.Log($"Moving towards {target}");
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
		}else{
			map[rowLoc, colLoc].whatsInside = "Empty";
		}

		rowLoc = (int)Math.Round(this.transform.position.x);
		colLoc = (int)Math.Round(this.transform.position.z);
		//Debug.Log($"Creature Pos {rowLoc}, {colLoc}");
		map[rowLoc, colLoc].whatsInside = "Creature";
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
		if (!lerping){
			yield return LerpPosition(target);
		}
        lerping = false;
	}
	
	void OnTriggerEnter(Collider other){
		//Debug.Log($"Collided with {other}");
		if(other.gameObject.CompareTag(berryBush.bushName)){
			if (mCurrentBehavior == behaviorState.FindingFood){
				bush = other.gameObject.GetComponentInParent<BerryBush>();
				if (!bush.isEaten){
					bush.isEaten = true;
					EatFood(bush);
				}
			}
		}
        else if ((other.gameObject.CompareTag(foodGoalName)) &&
                 (mCreatureName == "Fox")){
            
            Rabbit rabbit = other.gameObject.GetComponentInParent<Rabbit>();
            EatRabbit(rabbit);
        }
	}
	
	void Reproduce(){
        Debug.Log("Reproducing");
		mReproductiveUrge = 0;
        reproduce = true;
	}
	
	
	public void EatFood(BerryBush bush){
		if (bush != null){
			bush.EatBerries();
			mCurrentHunger -= 25;
			if (mCurrentHunger < 0) mCurrentHunger = 0;
		}
	}
	
	public void DrinkWater(){
		mCurrentThirst -= 50;
		if (mCurrentThirst < 0) mCurrentThirst = 0;
	}
    
    public void DontMove(){
        Debug.Log($"{mSex} is waiting");
        actions.Add(4);
        Debug.Log("Done Waiting");
    }
	
}
