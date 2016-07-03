using UnityEngine;
using System.IO;

public class RecordInfo {
    public string artist;
    public string name;
    public string frontPath;
    public string[] songs;

    public RecordInfo (string artist, string name, string frontPath, string[] songs) {
        this.artist = artist;
        this.name = name;
        this.frontPath = frontPath;
        this.songs = songs;
    }

    public string GetFullName () {
        return artist + " - " + name;
    }

    public static RecordInfo[] LoadDummyRecords (int amount) {
        string[] songs = new string[12];
        for (int i = 0; i < songs.Length; i++) {
            songs[i] = "Song #" + i;
        }

        RecordInfo[] records = new RecordInfo[amount];
        for (int i = 0; i < records.Length; i++) {
            records[i] = new RecordInfo("Temp Artist", "Temp Album #" + i, "", songs);
        }
        return records;
    }
}

public class Record : MonoBehaviour {
    public GameObject textRendererPrefab;
    public Transform meshTransform;
    public MeshRenderer meshRenderer;
    // This info will only be used in debug situations (actual infos are filled out at runtime)
    public RecordInfo info = new RecordInfo("The Recorders", "A Record", "", new string[] { "The Only Song" });
    public float spinTime = 0.25f;
    public float riseTime = 1f;
    public float lowerTime = 1f;
    public float riseHeight = 0.25f;

    private float meshHeight = 0;
    private float targetMeshHeight = 0;
    private float meshSpin = 0;
    private float targetMeshSpin = 0;

    void Start () {
        Texture2D frontTexture = LoadTexture(info.frontPath);
        if (frontTexture == null) {
            frontTexture = TextToTextureRenderer.RenderText(textRendererPrefab, info.artist + "\n" + info.name, new Color(1, 1, 1), new Color(0, 0, 0));
        }

        string songs = info.name + ":";
        for (int i = 0; i < info.songs.Length; i++) {
            songs += "\n" + i + ". " + info.songs[i];
        }
        Color backColor = Util.GetAverageColorFromTexture(frontTexture);
        Texture2D backTexture = TextToTextureRenderer.RenderText(textRendererPrefab, songs, backColor, 
            Util.GetBrightnessFromColor(backColor) < 0.5 ? new Color(1f, 1f, 1f) : new Color(0, 0, 0));

        // Material indices: 0 - back, 1 - front (cover)
        meshRenderer.materials[0].mainTexture = backTexture;
        meshRenderer.materials[1].mainTexture = frontTexture;
    }

    void Update () {
        if (meshHeight < targetMeshHeight) {
            meshHeight += riseHeight * Time.deltaTime / riseTime;
            if (meshHeight > targetMeshHeight) {
                meshHeight = targetMeshHeight;
            }
        }
        if (meshHeight > targetMeshHeight) {
            meshHeight -= riseHeight * Time.deltaTime / lowerTime;
            if (meshHeight < targetMeshHeight) {
                meshHeight = targetMeshHeight;
            }
        }
        if (meshSpin != targetMeshSpin) {
            float delta = targetMeshSpin - meshSpin;
            float sign = Mathf.Sign(delta);
            meshSpin += delta * Time.deltaTime / spinTime;
            if (targetMeshSpin - meshSpin < 0.5 || sign != Mathf.Sign(targetMeshSpin - meshSpin)) {
                meshSpin = targetMeshSpin;
            }
        }
        meshTransform.localPosition = new Vector3(0, meshHeight, 0);
        meshTransform.localEulerAngles = new Vector3(0, meshSpin, 0);
    }

    private Texture2D LoadTexture (string path) {
        string fullPath = Util.GetConfigValue("source") + path;
        Texture2D result = null;
        byte[] data;
        if (File.Exists(fullPath)) {
            data = File.ReadAllBytes(fullPath);
            result = new Texture2D(1, 1);
            result.LoadImage(data);
        }
        return result; 
    }

    public void Flip () {
        targetMeshSpin += 180;
    }

    public void SetSelected (bool selected) {
        targetMeshHeight = selected ? riseHeight : 0;
        if (!selected && targetMeshSpin % 360 != 0) {
            targetMeshSpin += 180;
        }
    }

    public bool IsSelected () {
        return riseHeight != 0 && targetMeshHeight == riseHeight;
    }
}
