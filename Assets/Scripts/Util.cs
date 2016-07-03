using UnityEngine;
using System.Collections;
using System.IO;

public class Util {
    private static bool configInitialized = false;
    private static Hashtable configs = new Hashtable();

    public static void InitializeConfig () {
        if (!configInitialized) {
            configInitialized = true;
            string path = "playmusic3d.cfg";
            if (File.Exists(path)) {
                string[] configContents = File.ReadAllLines(path);
                foreach (string entry in configContents) {
                    string[] pair = entry.Split('=');
                    configs.Add(pair[0], pair[1]);
                }
            } else {
                // TODO: Make a new config file
            }
        }
    }

    public static string GetConfigValue (string key) {
        InitializeConfig();
        return (string)configs[key];
    }

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

    public static float GetBrightnessFromColor (Color color) {
        return 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
    }

    public static Color GetOverlayColor (Color color) {
        return GetBrightnessFromColor(color) < 0.5 ? new Color(1, 1, 1) : new Color(0, 0, 0);
    }
}
