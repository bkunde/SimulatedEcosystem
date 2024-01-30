using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//takes noise map and turns it to texture appling to plane
public class MapDisplay : MonoBehaviour
{
	//get references 
	public Renderer textureRender;
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;
	
	//takes 2d float array and applies it
	public void DrawTexture(Texture2D texture){
		//applies texture to texture tenderer
		//sharedMaterial instances texture in the editor
		textureRender.sharedMaterial.mainTexture = texture;

		//sets plane size to map size
		textureRender.transform.localScale = new Vector3 (texture.width, 1, texture.height);
	}

	//creates mesh from meshData
	public void DrawMesh(MeshData meshData, Texture2D texture){
		meshFilter.sharedMesh = meshData.CreateMesh();
		meshRenderer.sharedMaterial.mainTexture = texture;

	}
}
