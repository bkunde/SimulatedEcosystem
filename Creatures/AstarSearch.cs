#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astar{

	public class AstarSearch{

		public string mGoal;
		
		List<Node> mReached;

		public AstarSearch(string goal, List<Node> reached){
			mGoal = goal;
			mReached = reached;
		}
		

		//AstarSearch
		public Node AstarSearchFunc(Node n0){
			Node n;
			State s;
			//List<Node> reached = new List<Node>();
			PriorityQueue q = new PriorityQueue();
			q.Put(n0);
			int count = 0;
			mReached.Add(n0);
			while (!(q.Empty())){
				//if (count >= 25){
					//return n0;
				//}
				//else{
					n = q.Get();
					s = n.mState;
					if (Model.GoalTest(s, mGoal)){
						return n;
					}
					List<int> actions = Model.Actions(s);
					PrintActions(actions);
					for (int i = 0; i < actions.Count; i++){
						int a = actions[i];
						State r = Model.Result(s, a);
						int g = Model.StepCost(s, a, r);
						int h = Model.Heuristic(s);
						if ((!CheckReached(r)) || (g < mReached[mReached.IndexOf(n)].mG)){
							//node = state, pnode, action, depth, f ,g
							Node s1 = new Node(r, n, a, n.mDepth+1, g+h, g);
							q.Put(s1);
							mReached.Add(s1);
						}else{}
					}
				count++;
				}
			//}
			return n0;
		}
		
		public bool CheckReached(State r){
			foreach(Node reachedNode in mReached){
				int x = reachedNode.mState.CreatureLoc.CreatureX;
				int y = reachedNode.mState.CreatureLoc.CreatureY;
				if (reachedNode.mState.subMapArray[x,y].cellLoc == 
					r.subMapArray[r.CreatureLoc.CreatureX, 
								  r.CreatureLoc.CreatureY].cellLoc){ 
					return true;
				}
			}
			return false;
		}
		public void PrintActions(List<int> actions){
			for (int i = 0; i < actions.Count; i++){
			}
		}
	}

	//State Class
	public class State : ICloneable{
		public Cell[,] subMapArray;
		public (int row, int col) mapLocation;
		public (int CreatureX, int CreatureY) CreatureLoc;
		
		public State(Cell[,] map, int sightRange, (int x,int y) location){
			if (sightRange > 0){
				CreateState(map, sightRange, location);
			}else{
				subMapArray = map;
				mapLocation = location;
			}
		}
		
		public void CreateState(Cell[,] map, int sightRange, (int x,int y) location){
			int startRow = location.x - sightRange;
			if (startRow < 0) startRow = 0;
			if (startRow > map.GetLength(0)) startRow = map.GetLength(0)-1;
			int startCol = location.y - sightRange;
			if (startCol < 0) startCol = 0;
			if (startCol > map.GetLength(1)) startCol = map.GetLength(1)-1;
			int numRows = location.x + sightRange;
			if (numRows < 0) numRows = 0;
			if (numRows > map.GetLength(0)) numRows = map.GetLength(0)-1;
			int numCols = location.y + sightRange;
			if (numCols < 0) numCols = 0;
			if (numCols > map.GetLength(1)) numCols = map.GetLength(1)-1;
			int subRowSize = Math.Abs(startRow-numRows)+1;
			int subColSize = Math.Abs(startCol-numCols)+1;
			subMapArray = new Cell[subRowSize, subColSize];

			for (int x = 0; x < (subRowSize); x++){
				for (int y = 0; y < (subColSize); y++){
					int newX = startRow + x;
					int newY = startCol + y;
					if (newX >= map.GetLength(0)) newX = map.GetLength(0)-1;
					if (newY >= map.GetLength(1)) newY = map.GetLength(1)-1;
					subMapArray[x,y] = map[newX, newY];
				}
			}
			CreatureLoc.CreatureX = location.x - startRow;
			CreatureLoc.CreatureY = location.y - startCol;
			mapLocation = location;
			/*
			for (int i = 0; i < map.GetLength(0); i++){
				for (int j = 0; j < map.GetLength(1); j++){
					if (map[i,j].whatsInside == "BerryBush")
				}
			}*/
		}
		// Clone method
		public object Clone(){
			// Create a new State object with the same properties
			State clonedState = new State(subMapArray, 0, mapLocation);

			// Copy the CreatureLoc values
			clonedState.CreatureLoc.CreatureX = CreatureLoc.CreatureX;
			clonedState.CreatureLoc.CreatureY = CreatureLoc.CreatureY;
			
			return clonedState;
		}
	}
	//Node Class
	public class Node{
		public State mState {get; set;}
		public Node? mPnode {get; set;}
		public int mAction {get; set;}
		public int mDepth {get; set;}
		public int mF {get; set;}
		public int mG {get; set;}

		
		public Node(State state, Node pnode, int action, int depth, int f, int g){
			mState = state;
			mPnode = pnode;
			mAction = action;
			mDepth = depth;
			mF = f;
			mG = g;
		}
	}

	//PriorityQueue
	public class PriorityQueue{
		private List<Node> queue = new List<Node>();
		
		public void Put(Node n){
			if (Empty()){
				queue.Add(n);
			}else{
				for(int i = 0; i < queue.Count; i++){
					if (queue[i].mF >= n.mF){
						queue.Insert(i, n);
						break;
					}
				}
			}
		}
		public Node Get(){
			Node n = queue[0];
			queue.RemoveAt(0);
			return n;
		}
		public bool Empty(){
			if (queue.Count == 0){
				return true;
			}else{
				return false;
			}	
		}
	}

	//Model
	public static class Model{
		public static List<int> Actions(State s){
			//returns list of available actions for state s
			List<int> actions = new List<int>(); 
			//bottom
			if (s.CreatureLoc.CreatureY == 0){
				actions.Add(0); //MoveUp
			}
			//top
			else if (s.CreatureLoc.CreatureY == (s.subMapArray.GetLength(1)-1)){
				actions.Add(2); //MoveDown
			}
			else{
				//if not on top or bottom row then creature can move up or down
				actions.Add(0);
				actions.Add(2);
			}
			//left
			if (s.CreatureLoc.CreatureX == 0){
				actions.Add(1); //MoveRight
			}
			//right
			else if (s.CreatureLoc.CreatureX == (s.subMapArray.GetLength(0)-1)){
				actions.Add(3); //MoveLeft
			}
			else{
				//if not on left or right then creature can move left or right
				actions.Add(1);
				actions.Add(3);
			}
			
			return actions;
		}	
		public static State Result(State s, int a){
			//returns the next state based on state s and action a
			State newState = (State)s.Clone();

			if (a == 0){ //Move up
				//if (newState.CreatureLoc.CreatureY < newState.subMapArray.GetLength(1)){
					//newState.subMapArray[newState.CreatureLoc.CreatureX, newState.CreatureLoc.CreatureX].whatsInside = "Empty";
					newState.CreatureLoc.CreatureY++;
					newState.mapLocation.col++;
					//newState.subMapArray[newState.CreatureLoc.CreatureX, newState.CreatureLoc.CreatureX].whatsInside = "Creature";
				//}
			}
			else if (a == 1){ //Move right
				//if (newState.CreatureLoc.CreatureX < newState.subMapArray.GetLength(0)){

					//Debug.Log("Moving right");

					//newState.subMapArray[newState.CreatureLoc.CreatureX, newState.CreatureLoc.CreatureX].whatsInside = "Empty";
					newState.CreatureLoc.CreatureX++;
					newState.mapLocation.row++;
					//newState.subMapArray[newState.CreatureLoc.CreatureX, newState.CreatureLoc.CreatureX].whatsInside = "Creature";
				//}
			}
			else if (a == 2){ //Move down
				//if (newState.CreatureLoc.CreatureY > 0){
					//newState.subMapArray[newState.CreatureLoc.CreatureX, newState.CreatureLoc.CreatureX].whatsInside = "Empty";
					newState.CreatureLoc.CreatureY--;
					newState.mapLocation.col--;
					//newState.subMapArray[newState.CreatureLoc.CreatureX, newState.CreatureLoc.CreatureX].whatsInside = "Creature";
				//}
			}
			else if (a == 3){ //Move left
				//if (newState.CreatureLoc.CreatureX > 0){
					//newState.subMapArray[newState.CreatureLoc.CreatureX, newState.CreatureLoc.CreatureX].whatsInside = "Empty";
					newState.CreatureLoc.CreatureX--;
					newState.mapLocation.row--;
					//newState.subMapArray[newState.CreatureLoc.CreatureX, newState.CreatureLoc.CreatureX].whatsInside = "Creature";
				//}
			}


			return newState;
		}
		public static bool GoalTest(State s, string goal){
			//returns T|F if the state s is a goal state
			if (s.subMapArray[s.CreatureLoc.CreatureX, s.CreatureLoc.CreatureY].whatsInside == goal){
				Debug.Log($"Goal at {s.CreatureLoc}");
				return true;
		}

			return false;
		}
		public static int StepCost(State s, int a, State s1){
			//StepCost
			if (s.subMapArray[s1.CreatureLoc.CreatureX, s1.CreatureLoc.CreatureY].whatsInside == "Water")
				return 5;

			return 1;
		}
		public static int Heuristic(State s){
			//ManhattanDist
			int goalX = -1; 
			int goalY = -1;
			for (int i = 0; i < s.subMapArray.GetLength(0); i++){
				for (int j = 0; j < s.subMapArray.GetLength(1); j++){
					if (s.subMapArray[i,j].whatsInside == "BerryBush"){
						goalX = i;
						goalY = j;
					}
				}
			}
			if (goalX != -1){
				int h = int.MaxValue;
				int row_dif = goalX - s.CreatureLoc.CreatureX;
				int col_dif = goalY - s.CreatureLoc.CreatureY;
				int sum = Math.Abs(row_dif)+Math.Abs(col_dif);
				if (sum < h)
					h = sum;
				return h;
			}
			return 1;
		}
	}
}
