using UnityEngine;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Threading;

public class Util : MonoBehaviour {
    public static string TEMP_SONG_PATH = ".temp-song.wav";
    public static string CONFIG_PATH = "playmusic3d.cfg";
    private static bool configInitialized = false;
    private static Hashtable configs = new Hashtable();

    private static bool cleanedUp = true;
    private static bool songLoaded = false;
    private static Thread loadingThread;

    private static Material blurringEffectMaterial;

    public static void InitializeConfig () {
        if (File.Exists(CONFIG_PATH)) {
            string[] configContents = File.ReadAllLines(CONFIG_PATH);
            foreach (string entry in configContents) {
                string[] pair = entry.Split('=');
                configs.Add(pair[0], pair[1]);
            }
            configInitialized = true;
        }
    }

    public static void CreateConfig (string config) {
        StreamWriter writer = new StreamWriter(File.Create(CONFIG_PATH));
        writer.WriteLine(config);
        writer.Close();
    }

    public static bool IsConfigInitialized () {
        return configInitialized;
    }

    public static string GetConfigValue (string key) {
        if (!IsConfigInitialized()) {
            InitializeConfig();
        }
        return (string)configs[key];
    }

    public static Texture2D LoadCoverFront (Album album) {
        if (album.coverFrontData.Length > 0) {
            Texture2D result = new Texture2D(1, 1);
            result.LoadImage(album.coverFrontData);
            return result;
        } else {
            Color bgColor = Random.ColorHSV();
            Color textColor = GetOverlayColor(bgColor);
            return TextToTextureRenderer.RenderTextWithColor(album.artist + "\n" + album.name, bgColor, textColor);
        }
    }

    public static Texture2D LoadCoverBack(Album album, Texture2D frontTexture) {
        if (album.coverBackData.Length > 0) {
            Texture2D result = new Texture2D(1, 1);
            result.LoadImage(album.coverBackData);
            return result;
        } else {
            string songs = "<i>" + album.name + (album.name != null ? "</i> by <i>" + album.artist + ":\n" : "") + "</i>";
            for (int i = 0; i < album.songs.Length; i++) {
                songs += (i + 1) + ". " + album.songs[i].name + "\n";
            }
            RenderTexture background = new RenderTexture(256, 256, -1);
            background.filterMode = FilterMode.Trilinear;
            blurringEffectMaterial.SetFloat("_Brightness", 0.4f);
            blurringEffectMaterial.SetFloat("_SamplingRange", 1f / 64f);
            Graphics.Blit(frontTexture, background, blurringEffectMaterial);

            return TextToTextureRenderer.RenderTextWithBackground(songs, background, new Color(1, 1, 1));
        }
    }

    public static void LoadTempSong (string path, bool panRight) {
        cleanedUp = false;
        songLoaded = false;
        loadingThread = new Thread(new ThreadStart(() => {
            Process process = new Process();
            process.StartInfo.FileName = "ffmpeg";
            string options = " -af \"pan=1c|c0=c" + (panRight ? "1" : "0") + "\" ";
            process.StartInfo.Arguments = "-i \"" + path + "\" -y -nostdin " + options + TEMP_SONG_PATH;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();
            process.WaitForExit();
            process.Close();

            File.SetAttributes(TEMP_SONG_PATH, FileAttributes.Hidden);
            songLoaded = true;
        }));
        loadingThread.Start();
    }

    public static bool HasSongBeenLoaded() {
        return songLoaded;
    }

    public static void CleanupTempSongThreaded () {
        Thread thread = new Thread(new ThreadStart(() => {
            CleanupTempSong();
        }));
        thread.Start();
    }

    public static void CleanupTempSong () {
        while (File.Exists(TEMP_SONG_PATH)) {
            try {
                File.Delete(TEMP_SONG_PATH);
                cleanedUp = true;
            } catch (IOException) {
            }
        }
    }

    public static bool TempSongCleanedUp () {
        return cleanedUp;
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

    public static GameObject GetHoveredGameObject () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 10);
        GameObject result = null;
        if (hit.collider != null) {
            result = hit.collider.gameObject;
        }
        return result;
    }

    public static void Log (string msg) {
        UnityEngine.Debug.Log(msg);
    }

    public static void Err (string msg) {
        UnityEngine.Debug.LogError(msg);    
    }

    /* Util non-static stuff */

    public Material blurringEffectMaterialInstance;

    void Start() {
        blurringEffectMaterial = blurringEffectMaterialInstance;
    }

    void OnApplicationQuit () {
        if (loadingThread != null && loadingThread.IsAlive) {
            loadingThread.Abort();
        }
        CleanupTempSong();
    }
}
