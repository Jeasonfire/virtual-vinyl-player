using UnityEngine;

public class RecordPlayer : Interactible {
    public CameraManager cam;
    public RecordPlayerAnimator animator;
    public Transform recordTransform;
    public AudioSource leftSpeaker;
    public AudioSource rightSpeaker;
    public AudioClip mainCrackle;

    private Record record;
    private int currentlyPlayingSongIndex = -1;

    public void Pause() {
        leftSpeaker.Pause();
        rightSpeaker.Pause();
    }

    public void UnPause() {
        leftSpeaker.UnPause();
        rightSpeaker.UnPause();
    }
    
    public void SetRecord(Record record) {
        record.MoveToTransform(recordTransform);
        record.StartLoadingAlbum();
        this.record = record;
    }

    public Record GetRecord() {
        return record;
    }

    /** <summary>
     * NOTE:
     * updatePositionInSong shouldn't be true if this is called multiple times 
     * in a second, because it causes crackling in the music. Occasionally setting
     * it to true, like when skipping by user input, is fine and not really noticeable. 
     * </summary>
     */
    public void PlaySongAt(float position, bool updatePositionInSong = false) {
        RecordSide recordSide = record.GetCurrentSide();
        int index = recordSide.GetClipIndex(position);
        bool changeSong = currentlyPlayingSongIndex != index;
        if (changeSong || updatePositionInSong) {
            float currentSongTime = position - recordSide.GetLengthUntil(index);
            leftSpeaker.time = currentSongTime;
            rightSpeaker.time = currentSongTime;
        }
        if (changeSong) {
            leftSpeaker.clip = record.GetCurrentSide().GetClipLeft(index);
            leftSpeaker.Play();
            rightSpeaker.clip = record.GetCurrentSide().GetClipRight(index);
            rightSpeaker.Play();
            currentlyPlayingSongIndex = index;
        }
    }

    public bool HasSongsToPlay(float position) {
        return record.GetCurrentSide().GetClipIndex(position) != -1;
    }

    public void SetPlaybackSpeed(float speed) {
        leftSpeaker.pitch = speed;
        rightSpeaker.pitch = speed;
    }

    public override void Interact() {
        if (Input.GetButton("Action (Secondary)")) {
            animator.FastForward();
        }
        animator.Update();
    }

    public override void StartInteracting() {
        cam.positionTweener.ClearMoves();
        cam.positionTweener.AddMove(cam.GetRelativePosition(transform.position, false, false, true), 0.3f);

        cam.rotationTweener.ClearMoves();
        cam.rotationTweener.AddMove(cam.GetRelativeRotation(new Vector3(45, 90, 0), true, true, false), 0.3f);

        cam.fovTweener.ClearMoves();
        cam.fovTweener.AddMove(40, 0.3f);
    }

    public override void StopInteracting() {
    }
}
