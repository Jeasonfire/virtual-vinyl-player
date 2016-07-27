using UnityEngine;
using System.Collections;

public class Record : MonoBehaviour {
	private static RecordData lastLoadedRecordData = null;

    public AudioClip[] crackles;
    public MeshRenderer meshRenderer;
    public TransformManager transformManager;

    private Transform playerTransform;
    private Transform originalParent;
    private bool onPlayer = false;

    public Album album;
	private RecordData recordData;

    void Start() {
        originalParent = transform.parent;
		recordData = new RecordData();
		if (RecordData.crackles == null) {
			RecordData.crackles = crackles;
		}
    }

    public void SetPlayerTransform(Transform transform) {
        playerTransform = transform;
    }

    public void MoveToPlayer() {
        if (playerTransform != null) {
            onPlayer = true;
            transform.parent = playerTransform;
            transformManager.positionTweener.position = transform.localPosition;
            transformManager.positionTweener.AddMove(transform.localPosition + new Vector3(0, 0, 2), 0.25f);
            transformManager.positionTweener.AddMove(new Vector3(0, 0, 0), 0.25f);
            transformManager.rotationTweener.AddMove(new Vector3(0, 0, 90), 0.25f);
            transformManager.rotationTweener.AddMove(new Vector3(0, 0, 0), 0.25f);
        }
    }

    public void TeleportToCase() {
        onPlayer = false;
        transform.parent = originalParent;
        transform.localPosition = new Vector3(0, 0.5f, 0);
        transform.localEulerAngles = new Vector3(0, 0, 90);
        transformManager.positionTweener.position = transform.localPosition;
        transformManager.positionTweener.ClearMoves();
        transformManager.rotationTweener.position = transform.localEulerAngles;
        transformManager.rotationTweener.ClearMoves();
    }

    public void MoveToCase() {
        if (!onPlayer) {
            transformManager.positionTweener.AddMove(new Vector3(0, 0.5f, 0), 0.25f);
            transformManager.rotationTweener.AddMove(new Vector3(0, 0, 90), 0.25f);
        }
    }

    public void MoveToDisplay() {
        if (!onPlayer) {
            transformManager.positionTweener.AddMove(new Vector3(0, 0.5f, -0.5f), 0.25f);
            transformManager.rotationTweener.AddMove(new Vector3(0, 0, 90), 0.25f);
        }
    }

	public RecordData GetRecordData() {
        return recordData;
    }

	public void LoadCoverArt() {
		meshRenderer.materials[1].mainTexture = Util.LoadCoverFront(album);
	}

	public void StartLoadingAlbum() {
		StartCoroutine("LoadAlbum");
	}

	private IEnumerator LoadAlbum() {
		if (lastLoadedRecordData != null && lastLoadedRecordData != recordData) {
			lastLoadedRecordData.UnloadSongs();
		}
		lastLoadedRecordData = recordData;

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

            recordData.AddSong(clips[0], clips[1]);
        }
    }
}
