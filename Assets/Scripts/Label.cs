using UnityEngine;
using System.Collections;

public class Label : MonoBehaviour {
    public GameObject textRendererPrefab;
    public MeshRenderer meshRenderer;
    public Color backgroundColor;
    public string labelText;

    void Start () {
        SetText(labelText);
    }

    public void SetText (string text) {
        labelText = text;
        meshRenderer.material.mainTexture = TextToTextureRenderer.RenderText(textRendererPrefab, labelText, backgroundColor);
    }
}
