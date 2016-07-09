using UnityEngine;
using System.Collections;

public class TextureSetter : MonoBehaviour {
    public Texture2D texture;

	void Start () {
        GetComponent<MeshRenderer>().material.mainTexture = texture;
	}
}
