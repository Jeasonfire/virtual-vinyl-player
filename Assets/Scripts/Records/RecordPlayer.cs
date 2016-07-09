using UnityEngine;

public class RecordPlayer : Interactible {
    public CameraManager cam;
    public RecordPlayerAnimator animator;
    public Transform recordTransform;
    public AudioSource leftSpeaker;
    public AudioSource rightSpeaker;

    private Record record;

    public void StartPlaying(float position) {
        RecordSide current = record.GetCurrentSide();
        leftSpeaker.clip = current.GetClip(0, false);
        rightSpeaker.clip = current.GetClip(0, true);
        leftSpeaker.Play();
        rightSpeaker.Play();
    }
    
    public void SetRecord(Record record) {
        record.MoveToTransform(recordTransform);
        record.StartLoadingAlbum();
        this.record = record;
    }

    public override void Interact() {
        animator.UpdateAnimations();
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
