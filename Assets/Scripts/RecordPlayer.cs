using UnityEngine;

public class RecordPlayer : MonoBehaviour {

    public CameraManager cam;
    public RecordsManager recordsManager;

    public HingeJoint spinnyThing;
    public Transform hand;
    public Transform handBase;

    public MeshRenderer recordRenderer;

    public AudioSource mainAudioSource;
    public AudioSource crackleAudioSource;
    public AudioClip[] songs;
    public AudioClip crackle;
    public AudioClip crackleLoop;
    public string status = "";

    public float handSpeed = 30f;
    public float handAngleStart = 30f;
    public float handAngleEnd = 60f;

    public float startSilence = 4.5f;

    private bool interacting = false;
    private float interactingStartedAt = 0;
    private bool justClicked = false;
    private bool justSkipped = false;

    private bool handUp = false;
    private bool playing = false;
    private bool prepared = false;
    private int currentSongIndex = 0;
    private float playedTime = 0;

	void Start () {
        crackleAudioSource.clip = crackleLoop;
        crackleAudioSource.loop = true;
        crackleAudioSource.volume = 0.2f;
    }

	void Update () {
        if (!interacting) {
            return;
        }
        if (Input.GetButton("Action (Tertiary)") && Time.time - interactingStartedAt > 0.3) {
            ChangeToRecordsManager();
        }
        if (songs == null || songs.Length == 0) {
            return;
        }

        if (!Input.GetButton("Action (Primary)")) {
            justClicked = false;
        }
        if (Input.GetButton("Action (Primary)") && !justClicked) {
            justClicked = true;
            if (!prepared) {
                SetHandUp();
                prepared = true;
                if (playing) {
                    SetSpinnyForce(0);
                    crackleAudioSource.Pause();
                    mainAudioSource.Pause();
                }
            } else {
                SetHandDown();
                prepared = false;
                if (HasHandReached(handAngleStart) && !playing) {
                    crackleAudioSource.PlayOneShot(crackle, 5f);
                    crackleAudioSource.PlayDelayed(crackle.length);
                    mainAudioSource.clip = songs[currentSongIndex];
                    while (mainAudioSource.clip == null && currentSongIndex < songs.Length) {
                        mainAudioSource.clip = NextSong();
                    }
                    mainAudioSource.PlayDelayed(startSilence);
                    SetSpinnyForce(100f);
                    playing = true;
                    playedTime = 0;
                } else {
                    if (playing) {
                        SetSpinnyForce(100f);
                        crackleAudioSource.UnPause();
                        mainAudioSource.UnPause();
                    }
                }
            }
        }

        if (justSkipped) {
            SetHandDown();
        }
        if (!Input.GetButton("Action (Secondary)")) {
            justSkipped = false;
        }
        if (Input.GetButton("Action (Secondary)") && !justSkipped) {
            justSkipped = true;
            SetHandUp();
            playedTime += GetCurrentSongLength() * (1f - GetCurrentSongProgress());
        }

        // Hand Y rotation
        if (prepared && !playing) {
            SetHandYRotation(handAngleStart);
            status = "starting";
        } else if (playing) {
            SetHandYRotation(handAngleStart + (handAngleEnd - handAngleStart) * GetSongsProgress());
            status = "playing";
        } else {
            SetHandYRotation(0);
            status = "returning";
        }

        // Hand Z rotation
        if (handUp) {
            SetHandZRotation(170f);
        } else {
            SetHandZRotation(180f);
        }

        // Hand rotation fix (to fix theautomatic messing around with angles that go negative)
        // Y
        Vector3 nr = handBase.eulerAngles;
        if (nr.y > 180) {
            nr.y = (nr.y % 360) - 360;
        }
        nr.y = Mathf.Clamp(nr.y, 0, handAngleEnd);
        handBase.eulerAngles = nr;
        // Z
        nr = hand.eulerAngles;
        nr.z = Mathf.Clamp(nr.z, 170, 180);
        hand.eulerAngles = nr;

        // Song timer
        if (mainAudioSource.isPlaying) {
            playedTime += Time.deltaTime;
        }

        // Song ending handling
        if (HasFinished() && playing) {
            AudioClip nextSong = NextSong();
            if (nextSong != null) {
                mainAudioSource.Stop();
                mainAudioSource.clip = nextSong;
                while (mainAudioSource.clip == null && currentSongIndex < songs.Length) {
                    mainAudioSource.clip = NextSong();
                }
                mainAudioSource.Play();
            } else {
                SetSpinnyForce(0);
                SetHandUp();
                mainAudioSource.Stop();
                crackleAudioSource.Stop();
                playing = false;
                prepared = false;
                playedTime = 0;
                currentSongIndex = 0;
            }
        }
        if (!prepared && !playing && HasHandReached(0)) {
            SetHandDown();
        }
    }
    
    void ChangeToRecordsManager () {
        interacting = false;
        recordsManager.StartInteracting();
    }

    public void StartInteracting () {
        interacting = true;
        interactingStartedAt = Time.time;
        cam.targetPosition = cam.GetDefaultPosition();
        cam.targetPosition.z = transform.position.z;
        cam.targetRotation = cam.GetDefaultRotation();
        cam.targetRotation.y = 90;
        cam.targetRotation.x = 49;
        cam.targetFov = 28;
    }

    float GetCurrentSongLength () {
        return songs[currentSongIndex] == null ? 0 : songs[currentSongIndex].length;
    }

    float GetSongsLength () {
        float total = startSilence;
        for (int i = 0; i < songs.Length; i++) {
            if (songs[i] != null) {
                total += songs[i].length;
            }
        }
        return total;
    }

    float GetCurrentSongProgress() {
        return (playedTime - TimeSpentOnLastSongs()) / GetCurrentSongLength();
    }

    float GetSongsProgress () {
        return playedTime / GetSongsLength();
    }

    float TimeSpentOnLastSongs () {
        float total = startSilence;
        for (int i = 0; i < currentSongIndex; i++) {
            if (songs[i] != null) {
                total += songs[i].length;
            }
        }
        return total;
    }

    bool HasFinished () {
        return GetCurrentSongProgress() >= 1 || !playing;
    }

    bool HasHandReached (float target) {
        return Mathf.Abs(target - handBase.eulerAngles.y) < 0.1;
    }

    AudioClip NextSong() {
        if (currentSongIndex + 1 >= songs.Length) {
            return null;
        } else {
            return songs[++currentSongIndex];
        }
    }

    void SetHandUp () {
        handUp = true;
    }

    void SetHandDown () {
        handUp = false;
    }

    void SetSpinnyForce (float force) {
        JointMotor tempMotor = spinnyThing.motor;
        tempMotor.force = force;
        spinnyThing.motor = tempMotor;
    }

    void SetHandZRotation(float target) {
        if (target == hand.eulerAngles.z) {
            return;
        }
        Vector3 newRotation = hand.eulerAngles;
        float dir = Mathf.Sign(target - newRotation.z);
        newRotation.z = Mathf.Clamp(hand.eulerAngles.z + handSpeed * dir * Time.deltaTime,
            dir > 0 ? 0 : target, dir > 0 ? target : newRotation.z);
        hand.eulerAngles = newRotation;
    }

    void SetHandYRotation (float target) {
        if (target == handBase.eulerAngles.y) {
            return;
        }
        Vector3 newRotation = handBase.eulerAngles;
        float dir = Mathf.Sign(target - newRotation.y);
        newRotation.y = Mathf.Clamp(handBase.eulerAngles.y + handSpeed * dir * Time.deltaTime, 
            dir > 0 ? 0 : target, dir > 0 ? target : newRotation.y);
        handBase.eulerAngles = newRotation;
    }

    public void LoadSong (RecordInfo record) {
        // Load songs
        songs = new AudioClip[record.songs.Length];
        for (int i = 0; i < record.songs.Length; i++) {
            string path = record.songs[i].path;
            Util.LoadTempSong(path);
            WWW www = new WWW("file://" + Util.TEMP_SONG_PATH);
            songs[i] = www.GetAudioClip(true);
            // Hang until loading is done
            while (songs[i].loadState != AudioDataLoadState.Loaded) {
                if (www.error != null && www.error.Length > 0) {
                    Debug.LogError(www.error);
                    return;
                }
            }
            // Hang until stuff is deleted
            Util.CleanupTempSong();
        }
        Util.CleanupTempSong();

        // Load art
        recordRenderer.materials[1].mainTexture = Util.LoadTexture(record);
    }
}
