using UnityEngine;

public class Util {
    public static Color GetAverageColorFromTexture (Texture2D texture, float samplesX = 8, float samplesY = 8) {
        float totalR = 0;
        float totalG = 0;
        float totalB = 0;
        float count = samplesX * samplesY;
        for (float y = 0; y < samplesY; y++) {
            for (float x = 0; x < samplesX; x++) {
                Color pixel = texture.GetPixelBilinear(x / samplesX, y / samplesY);
                totalR += pixel.r;
                totalG += pixel.g;
                totalB += pixel.b;
            }
        }
        return new Color(totalR / count, totalG / count, totalB / count);
    }
}
