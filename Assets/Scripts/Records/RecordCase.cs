using UnityEngine;
using System.IO;

public class RecordCase : MonoBehaviour {
    public Transform meshTransform;
    public MeshRenderer meshRenderer;
    public TextMesh playText;
    public Rigidbody body;
    public AudioSource audioSource;
    public AudioClip[] slapSounds;
    public AudioClip[] wooshSounds;

    // This album instance will only be used in debug situations (actual albums are filled out at runtime)
    public Album album = new Album("The Recorders", "A Record", new byte[0], new Song[0]);
    public float spinTime = 0.25f;
    public float riseTime = 1f;
    public float lowerTime = 1f;
    public float riseHeight = 0.25f;

    private float meshHeight = 0;
    private float targetMeshHeight = 0;
    private float meshSpin = 0;
    private float targetMeshSpin = 0;

    void Start () {
        Texture2D frontTexture = Util.LoadAlbumArt(album);
        if (frontTexture == null) {
        }

        string songs = "<i>" + album.name + (album.name != null ? "</i> by <i>" + album.artist + ":\n" : "") + "</i>";
        for (int i = 0; i < album.songs.Length; i++) {
            songs += (i + 1) + ". " + album.songs[i].name + "\n";
        }
        Color backColor = Util.GetAverageColorFromTexture(frontTexture);
        Texture2D backTexture = TextToTextureRenderer.RenderText(songs, backColor, Util.GetOverlayColor(backColor));

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

    public void PlaySlapSound (float volume = 1f) {
        AudioClip clip = slapSounds[Random.Range(0, slapSounds.Length)];
        audioSource.PlayOneShot(clip, 0.1f * volume);
    }

    public void PlayWooshSound(float volume = 1f) {
        AudioClip clip = wooshSounds[Random.Range(0, wooshSounds.Length)];
        audioSource.PlayOneShot(clip, 0.025f * volume);
    }

    public void Flip () {
        PlayWooshSound(4f);
        targetMeshSpin += 180;
    }

    public void SetSelected (bool selected) {
        targetMeshHeight = selected ? riseHeight : 0;
        if (!selected && targetMeshSpin % 360 != 0) {
            Flip();
        } else {
            PlayWooshSound(3f);
        }
    }

    public bool IsSelected () {
        return riseHeight != 0 && targetMeshHeight == riseHeight;
    }

    void OnCollisionEnter (Collision collision) {
        Vector3 highestPoint = new Vector3();
        foreach (ContactPoint contactPoint in collision.contacts) {
            if (highestPoint.y < contactPoint.point.y) {
                highestPoint = contactPoint.point;
            }
        }
        highestPoint.z = transform.position.z;
        audioSource.transform.position = highestPoint;
        PlaySlapSound(Mathf.Min(Mathf.Pow(collision.relativeVelocity.magnitude / 1.5f, 2), 2f));
    }
}