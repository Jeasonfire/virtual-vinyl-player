using UnityEngine;
using System.Collections;

public class TextToTextureRenderer : MonoBehaviour {
    private static TextToTextureRenderer textRenderer;

    public RenderTexture textTexture;
    public Camera textCamera;
    public TextMesh textMesh;

    public static Texture2D RenderText (string text, Color bgColor, Color textColor) {
        if (textRenderer == null) {
            textRenderer = GameObject.Find("TextToTextureRenderer").GetComponent<TextToTextureRenderer>();
        }
        textRenderer.textCamera.backgroundColor = bgColor;
        textRenderer.textMesh.GetComponent<MeshRenderer>().material.color = textColor;
        textRenderer.textMesh.text = text;
        textRenderer.textCamera.Render();

        int texWidth = textRenderer.textTexture.width;
        int texHeight = textRenderer.textTexture.height;
        Texture2D result = new Texture2D(texWidth, texHeight);
        RenderTexture previouslyActive = RenderTexture.active;
        RenderTexture.active = textRenderer.textTexture;
        result.ReadPixels(new Rect(0, 0, texWidth, texHeight), 0, 0);
        result.Apply();
        RenderTexture.active = previouslyActive;

        return result;
    }
}
