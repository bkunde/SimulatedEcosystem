using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator{

	public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height){
		Texture2D texture = new Texture2D(width, height);
		//fix bluriness
		texture.filterMode = FilterMode.Point;
		//fix wrapping
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels (colorMap);
		texture.Apply();
		return texture;
	}

	public static Texture2D TextureFromHeightMap(float[,] heightMap){
		//get dimensions of map
		int width = heightMap.GetLength (0);
		int height = heightMap.GetLength (1);

		//create array of colors for every pixel in texture
		Color[] colorMap = new Color[width * height];
		//loop thru every value in heightMap 
		for (int y = 0; y < height; y++){
			for (int x = 0; x < height; x++){
				//set value of each color
				//Lerp takes a start point and end point and a values that interpolates between those points
				colorMap [y*width+x] = Color.Lerp(Color.black, Color.white, heightMap[x,y]);
			}
		}
		return TextureFromColorMap(colorMap, width, height);
	}
}
