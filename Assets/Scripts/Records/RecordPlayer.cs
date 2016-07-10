using UnityEngine;

public class RecordPlayer : Interactible {
    public CameraManager cam;
    public RecordPlayerAnimator animator;
    public Transform recordTransform;
    public AudioSource leftSpeaker;
    public AudioSource rightSpeaker;
    public AudioClip mainCrackle;

    private Record record;
    private bool shouldBePlaying = false;
    private int songIndex = 0;

    public void StartPlaying() {
        PlayNextSong();
        shouldBePlaying = true;
    }
    
    public void SetRecord(Record record) {
        record.MoveToTransform(recordTransform);
        record.StartLoadingAlbum();
        this.record = record;
    }

    private void PlayNextSong() {
        leftSpeaker.clip = record.GetCurrentSide().GetClip(songIndex, false);
        leftSpeaker.Play();
        rightSpeaker.clip = record.GetCurrentSide().GetClip(songIndex, true);
        rightSpeaker.Play();
        songIndex++;
    }

    private void SetPlaybackSpeed(float speed) {
        leftSpeaker.pitch = speed;
        rightSpeaker.pitch = speed;
    }

    public override void Interact() {
        animator.UpdateAnimations();
        if (!leftSpeaker.isPlaying && shouldBePlaying) {
            PlayNextSong();
        }
        if (shouldBePlaying) {
            SetPlaybackSpeed(animator.spinnyThingWatcher.GetSpeedRatio());
        }
    }

    public override void StartInteracting() {
        cam.positionTweener.ClearMoves();
        cam.positionTweener.AddMove(cam.GetRelativePosition(transform.position, false, false, true), 0.3f);

        cam.rotationTweener.ClearMoves();
        cam.rotationTweener.AddMove(cam.GetRelativeRotation(new Vector3(49, 90, 0), true, true, false), 0.3f);

        cam.fovTweener.ClearMoves();
        cam.fovTweener.AddMove(30, 0.3f);
    }

    public override void StopInteracting() {
    }
}
