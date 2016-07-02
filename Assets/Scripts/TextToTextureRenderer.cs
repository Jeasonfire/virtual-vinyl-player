using UnityEngine;
using System.Collections;

public class TextToTextureRenderer : MonoBehaviour {
    private static TextToTextureRenderer textRenderer;

    public RenderTexture textTexture;
    public Camera textCamera;
    public TextMesh textMesh;

    public static Texture RenderText (GameObject textRendererPrefab, string text, float widthRatio = 1.0f) {
        if (textRenderer == null) {
            textRenderer = Instantiate<GameObject>(textRendererPrefab).GetComponent<TextToTextureRenderer>();
        }
        textRenderer.textMesh.text = text;
        textRenderer.textCamera.Render();

        RenderTexture result = new RenderTexture(textRenderer.textTexture.width,
            textRenderer.textTexture.height, textRenderer.textTexture.depth, textRenderer.textTexture.format);
        Graphics.Blit(textRenderer.textTexture, result);
        return result;
    }
}
