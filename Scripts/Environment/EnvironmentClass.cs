using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentClass : MonoBehaviour
{
    [SerializeField]
    private bool DEBUG = false;
    private Canvas tileCanvas;
    private bool fileWrote = false;

    public int mapSize = 25;
    //Enviromental Parameters

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
    public DataCollection dc;
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

	public List<BerryBush> bushes = new List<BerryBush>();

    public GameObject treeHolder; 
    public GameObject bushHolder; 
    public GameObject rockHolder; 

    Cell[,] map;
	BerryBush bush;
    TreeCreator tree;
    RockCreator rock;
	Rabbit rabbit;
    Fox fox;

    TextMesh textMesh;

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
        if (DEBUG){
            ShowCell();
        }
    }

	void Update(){
        List<Rabbit> newRabbits = new List<Rabbit>(); 
        List<Fox> newFoxes = new List<Fox>(); 
        
        List<Rabbit> deadRabbits = new List<Rabbit>(); 
        List<Fox> deadFoxes = new List<Fox>(); 
		foreach(Rabbit r in rabbits){
            Rabbit kit;
            if (r.isDead){
                deadRabbits.Add(r);
                continue;
            }
			if (r != null){
                if(r.reproduce){
                    r.reproduce = false;
                    int litter = UnityEngine.Random.Range(1, 4);
                    for (int i = 0; i < litter; i++){
                        kit = CreateNewRabbit(r.rowLoc, r.colLoc, r);
                        newRabbits.Add(kit);
                    }
                }
                r.UpdateCreature();
            }
		}
        foreach(Fox f in foxes){
            Fox pup;
            if (f.isDead){
                deadFoxes.Add(f);
                continue;
            }
			if (f != null){
                if(f.reproduce){
                    pup = CreateNewFox(f.rowLoc, f.colLoc, f);
                    newFoxes.Add(pup);
                }
				f.UpdateCreature();
            }
        }
        
        if (DEBUG){
            for (int x = 0; x < map.GetLength(0); x++){
                for (int y = 0; y < map.GetLength(1); y++){
                    tile = map[x,y].terrianTile;
                    textMesh = tile.GetComponentsInChildren<TextMesh>()[0];
                    textMesh.text = map[x,y].whatsInside;
                }
            }   
        }
		foreach(Rabbit kit in newRabbits){
            rabbits.Add(kit);
        }
        foreach(Fox pup in newFoxes){
            foxes.Add(pup);
        }

		foreach(Rabbit rabbit in deadRabbits){
            rabbits.Remove(rabbit);
        }
        foreach(Fox fox in deadFoxes){
            foxes.Remove(fox);
        }
        if (((rabbits.Count == 0) && (foxes.Count == 0)) || (Input.GetKeyDown("space"))){
            if (!(fileWrote)){
                Debug.Log("Writing File");
                dc.WriteFile();
                fileWrote = true;
            }
        }
	}
	
	void PlaceBushes(){
		BerryBush newbush;
		for (int i = 0; i < bushAmount; i++){
			newbush = Instantiate(bush);
			newbush.CreateBush();
			bushes.Add(newbush);
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
    Rabbit CreateNewRabbit(int x, int y, Rabbit r){
		Rabbit newRabbit;
        newRabbit = Instantiate(rabbit);
        newRabbit.CreateRabbit(x, y, r);
        return newRabbit;
    }
	void PlaceFoxes(){
		Fox newFox;
		for (int i = 0; i < foxAmount; i++){
			newFox = Instantiate(fox);
			newFox.CreateStartFox();
			foxes.Add(newFox);
		}
	}
    Fox CreateNewFox(int x, int y, Fox f){
		Fox newFox;
        newFox = Instantiate(fox);
        newFox.CreateFox(x, y, f);
        return newFox;
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
						if((newY<mapSize-2)&&(0<newX)){
							newY++;
							newX--;
						}
					}else if(r == 1){//up
						if(newY<mapSize-2)
							newY++;
					}else if(r == 2){//topright
						if((newY<mapSize-2)&&(newX<mapSize-2)){
							newY++;
							newX++;
						}
					}else if(r == 3){//left
						if(newX>0)
							newX--;
					}else if(r == 4){//right
						if(newX<mapSize-2)
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
						if((newY>0)&&(newX<mapSize-2)){
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
    
    void ShowCell(){
        GameObject tilePiece;
        GameObject text;
        for (int x = 0; x < map.GetLength(0); x++){
            for (int y = 0; y < map.GetLength(1); y++){
                tilePiece = map[x,y].terrianTile;
                
                text = new GameObject();
                text.transform.parent = tilePiece.transform;

                text.transform.position = new Vector3(x, 0.2f, y);
                text.transform.Rotate(90,0,0);
                text.transform.localScale = new Vector3(1,1,1);

                textMesh = text.AddComponent<TextMesh>();
                textMesh.text = map[x,y].whatsInside;
                textMesh.fontSize = 10;
                
                
            }
        }
    }
}


[System.Serializable]
public struct TerrainType{
	public string name;
	public Color color;
}

