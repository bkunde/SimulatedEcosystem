using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//No need for monoBehaviour as no object gets this script applied to them
//Multpile instances of script are NOT going to be created so static is used
public static class Noise{
	
	//generate a noise map that returns 2dArray of values between 0-1
	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset){
		//create 2dFloat Array of size W x H
		float[,] noiseMap = new float[mapWidth, mapHeight];
		//seed generator
		System.Random prng = new System.Random (seed);
		Vector2[] octaveOffsets = new Vector2[octaves];
		for (int i = 0; i < octaves; i++){
			float offsetX = prng.Next(-100000, 100000) + offset.x; //range can't be too large or else perlinNoise will return same value everytime
			float offsetY = prng.Next(-100000, 100000) + offset.y; 
			octaveOffsets[i] = new Vector2 (offsetX, offsetY);
		}
		//check that scale is not 0
		if (scale <= 0){
			scale = 0.0001f;
		}

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;
		//loop thru array
		for (int y = 0; y < mapHeight; y++){
			for (int x = 0; x < mapWidth; x++){

				float amplitude = 1;
				float frequency = 1;	//higher frequency gives further apart sample points
				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++){
					//create sample values that are not integer values
					//integer values will give us the same perlinValue
					float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[i].x;
					float sampleY = (y-halfHeight) /scale * frequency + octaveOffsets[i].y;
					//get perlinValue
					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY)*2-1; //the *2-1 gives a range of -1 to 1
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if (noiseHeight > maxNoiseHeight)
					maxNoiseHeight = noiseHeight;
				else if (noiseHeight < minNoiseHeight)
					minNoiseHeight = noiseHeight;

				noiseMap[x, y] = noiseHeight;
			}
		}
		//sets noiseMap values to points between the min and max range i.e normailizes the noiseMap
		for (int y = 0; y < mapHeight; y++){
			for (int x = 0; x < mapWidth; x++){
				noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
			}
		}
		return noiseMap;
	}


}

