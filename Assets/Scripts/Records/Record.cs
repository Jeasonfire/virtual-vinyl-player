using UnityEngine;
using System.Collections;

public class Record : MonoBehaviour {
    public AudioClip mainCrackle;
    public AudioClip[] crackles;
    public MeshRenderer meshRenderer;

    private Transform originalTransform;
    
    public Album album;
    private RecordSide[] sides;
    private int loadingSide = 0;
    private int listeningSide = 0;

    void Start() {
        originalTransform = transform;
        sides = new RecordSide[] {
            new RecordSide(mainCrackle, crackles),
            new RecordSide(mainCrackle, crackles)
        };
    }

    public void ReturnToOriginalTransform() {
        MoveToTransform(originalTransform);
    }

    public void MoveToTransform(Transform t) {
        transform.parent = t.parent;
        transform.localPosition = t.localPosition;
        transform.localRotation = t.localRotation;
        transform.localScale = t.localScale;
    }

    public RecordSide GetCurrentSide() {
        return sides[listeningSide];
    }

    public void StartLoadingAlbum() {
        StartCoroutine("LoadAlbum");
    }

    private IEnumerator LoadAlbum() {
        // Load cover art
        meshRenderer.materials[1].mainTexture = Util.LoadAlbumArt(album);

        // Load songs
        for (int i = 0; i < album.songs.Length; i++) {
            // Progress
            string path = album.songs[i].path;

            AudioClip[] clips = new AudioClip[2];
            for (int j = 0; j < 2; j++) {
                // Create temp file
                Util.LoadTempSong(path, j == 1);
                while (!Util.HasSongBeenLoaded()) {
                    yield return null;
                }
                
                // Load from  temp file
                WWW www = new WWW("file://" + Util.TEMP_SONG_PATH);
                clips[j] = www.GetAudioClip(true, false, AudioType.WAV);
                while (clips[j].loadState != AudioDataLoadState.Loaded) {
                    if (www.error != null) {
                        Debug.LogError("WWW Error while loading '" + path + "':\n" + www.error);
                    }
                    yield return null;
                }
                clips[j].name = album.songs[i].artist + " - " + album.songs[i].name;
                
                Util.CleanupTempSongThreaded();
                while (!Util.TempSongCleanedUp()) {
                    yield return null;
                }
            }
            if (sides[loadingSide].GetLoadedLength() + clips[0].length > sides[loadingSide].GetLength()) {
                loadingSide = 1;
            }
            sides[loadingSide].AddSong(clips[0], false);
            sides[loadingSide].AddSong(clips[1], true);
        }
    }
}
