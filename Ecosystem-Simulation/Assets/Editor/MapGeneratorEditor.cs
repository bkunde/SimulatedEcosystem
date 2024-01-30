using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]
public class MapGeneratorEditor : Editor{

	public override void OnInspectorGUI() {
		//get Ref to mapGenerator
		MapGenerator mapGen = (MapGenerator)target;

		//if anyvalue was changed
		if (DrawDefaultInspector ()){
			if (mapGen.autoUpdate) {
				mapGen.GenerateMap();
			}
		}

		if (GUILayout.Button("Generate")){
			mapGen.GenerateMap();
		}
	}
}
