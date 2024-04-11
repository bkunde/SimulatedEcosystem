using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentClass : MonoBehaviour
{
    public int mapSize = 25;
    //Enviromental Parameters
    //BiomeType

	[Range(0,1)]
	public float waterRatio;
	[Range(0,1)]
	public float poolingPercent = 0.9f;
	public int bushAmount = 3;
    public int treeAmount = 10;
    public int rockAmount = 15;
	public int rabbitAmount = 1;
    public int foxAmount = 1;
    //MapArray
    public MapArray mapArray;
    public GameObject tile;
	public BerryBush berryBush;
    public TreeCreator treeCreator;
    public RockCreator rockCreator;
	public Rabbit rabbitClass;
    public Fox foxClass;
    public TerrainType[] regions;

	public List<Rabbit> rabbits = new List<Rabbit>();
	public List<Fox> foxes = new List<Fox>();

    public GameObject treeHolder; 
    public GameObject bushHolder; 
    public GameObject rockHolder; 

    Cell[,] map;
	BerryBush bush;
    TreeCreator tree;
    RockCreator rock;
	Rabbit rabbit;
    Fox fox;

    void Start(){
		map = mapArray.mapArray;
		bush = berryBush;
        tree = treeCreator;
        rock = rockCreator;
		rabbit = rabbitClass;
        fox = foxClass;
		InitializeMap();
		PlaceBushes();
        PlaceTrees();
        PlaceRocks();
		PlaceRabbits();
		PlaceFoxes();
    }

	void Update(){
		foreach(Rabbit r in rabbits){
            if (r.isDead){
                rabbits.Remove(r);
                break;
            }
			if (!(r is null)){
                if(r.reproduce){
                    CreateNewRabbit(r.rowLoc, r.colLoc);
                }
				r.UpdateCreature();
            }
		}
        foreach(Fox f in foxes){
            if (f.isDead){
                foxes.Remove(f);
                break;
            }
			if (!(f is null)){
                if(f.reproduce){
                    CreateNewRabbit(f.rowLoc, f.colLoc);
                }
				f.UpdateCreature();
            }
        }
	}
	
	void PlaceBushes(){
		BerryBush newbush;
		for (int i = 0; i < bushAmount; i++){
			newbush = Instantiate(bush);
			newbush.CreateBush();
            newbush.transform.parent = bushHolder.transform;
		}
	}
	
	void PlaceTrees(){
		TreeCreator newTree;
		for (int i = 0; i < treeAmount; i++){
			newTree = Instantiate(tree);
			newTree.CreateTree();
            newTree.transform.parent = treeHolder.transform;
		}
	}

	void PlaceRocks(){
		RockCreator newRock;
		for (int i = 0; i < rockAmount; i++){
			newRock = Instantiate(rock);
			newRock.CreateRock();
            newRock.transform.parent = rockHolder.transform;
		}
	}
	void PlaceRabbits(){
		Rabbit newRabbit;
		for (int i = 0; i < rabbitAmount; i++){
			newRabbit = Instantiate(rabbit);
			newRabbit.CreateStartRabbit();
			rabbits.Add(newRabbit);
		}
	}
    void CreateNewRabbit(int x, int y){
		Rabbit newRabbit;
        newRabbit = Instantiate(rabbit);
        newRabbit.CreateRabbit(x, y);
        rabbits.Add(newRabbit);
    }
	void PlaceFoxes(){
		Fox newFox;
		for (int i = 0; i < foxAmount; i++){
			newFox = Instantiate(fox);
			newFox.CreateStartFox();
			foxes.Add(newFox);
		}
	}
    void CreateNewFox(int x, int y){
		Fox newFox;
        newFox = Instantiate(fox);
        newFox.CreateFox(x, y);
        foxes.Add(newFox);
    }


    void InitializeMap(){
		int waterAmount = (int)((mapSize*mapSize) * waterRatio);
		int landAmount = (mapSize*mapSize) - waterAmount;

		int x = UnityEngine.Random.Range(0, mapSize-1);
		int y = UnityEngine.Random.Range(0, mapSize-1);
		if (map[x,y].Name == "NameNotSet"){
			generateWaterTile(x,y);
			waterAmount--;
		}
		int cX = x;
		int cY = y;
		//generateMap
		int r = 0;
		while(waterAmount > 0){
			System.Random random = new System.Random();
			float randNum = (float)random.NextDouble();
			if (randNum < poolingPercent){
				//pool
				int newX = cX;
				int newY = cY;
				while (map[newX,newY].Name == "Water"){
					if (r > 7){
						break;
					}
					//int r = UnitnewYEngine.Random.Range(0,7);
					if (r == 0){//topleft
						if((newY<mapSize-1)&&(0<newX)){
							newY++;
							newX--;
						}
					}else if(r == 1){//up
						if(newX<mapSize-1)
							newY++;
					}else if(r == 2){//topright
						if((newY<mapSize-1)&&(newX<mapSize-1)){
							newY++;
							newX++;
						}
					}else if(r == 3){//left
						if(newX>0)
							newX--;
					}else if(r == 4){//right
						if(newX<mapSize-1)
							newX++;
					}else if (r == 5){//bottomleft
						if((newY>0)&&(newX>0)){
							newX--;
							newY--;
						}
					}else if (r == 6){//down
						if(newY>0)
							newY--;
					}else if ( r == 7){//bottomright
						if((newY>0)&&(newX<mapSize-1)){
							newY--;
							newX++;
						}
					}
					r++;
				}
				if(r<=7){
					generateWaterTile(newX,newY);
					waterAmount--;
				}else{
					r = 0;
					int oldX = cX;
					int oldY = cY;
					if(oldX<=0){oldX=1;}
					if(oldY<=0){oldY=1;}
					if(oldX>=mapSize-1){oldX=mapSize-2;}
					if(oldY>=mapSize-1){oldY=mapSize-2;}
					cX = UnityEngine.Random.Range(oldX-1, oldX+1);
					cY = UnityEngine.Random.Range(oldY-1, oldY+1);
				}
			}else{
				//new loc
				x = UnityEngine.Random.Range(0, mapSize);
				y = UnityEngine.Random.Range(0, mapSize);
				if (map[x,y].Name == "NameNotSet"){
					generateWaterTile(x,y);
					waterAmount--;
					cX = x;
					cY = y;
				}
			}
		}

		while(landAmount > 0){
			x = UnityEngine.Random.Range(0, mapSize);
			y = UnityEngine.Random.Range(0, mapSize);
			if (map[x,y].Name == "NameNotSet"){
				generateLandTile(x,y);
				landAmount--;
			}
		}
	}

	void generateLandTile(int x, int y){
		tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
		tile.transform.parent = this.transform;
		tile.transform.position = new Vector3(x, 0, y);
		tile.transform.localScale = new Vector3(0.1f, 1, 0.1f);
		tile.GetComponent<Renderer>().material.color = regions[0].color;
		(int x, int y) loc = (x, y);
		map[x,y].Name = "Grass";
		map[x,y].terrianTile = tile;
		map[x,y].cellLoc = loc;
	}
	void generateWaterTile(int x, int y){
		tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
		tile.transform.parent = this.transform;
		tile.transform.position = new Vector3(x, 0, y);
		tile.transform.localScale = new Vector3(0.1f, 1, 0.1f);
		tile.GetComponent<Renderer>().material.color = regions[1].color;
		(int x, int y) loc = (x, y);
		map[x,y].Name = "Water";
		map[x,y].terrianTile = tile;
		map[x,y].whatsInside = "Water";
		map[x,y].cellLoc = loc;
	}
}

[System.Serializable]
public struct TerrainType{
	public string name;
	public Color color;
}

