using UnityEngine;
using System;

[Serializable]
public class RecordPlayerAnimationProperties {
    [Range(0.01f, 2)]
    public float interactionTransitionLength = 1;
}

public class RecordPlayer : Interactible {
    public CameraManager cam;
    public RecordPlayerAnimator animator;
    public RecordPlayerAnimationProperties animProps;
    public Transform recordTransform;
    public AudioSource leftSpeaker;
    public AudioSource rightSpeaker;

    private Record record;

    public void StartPlaying(float position) {
        animator.StartPlaying();
        RecordSide current = record.GetCurrentSide();
        leftSpeaker.clip = current.GetClip(0, false);
        rightSpeaker.clip = current.GetClip(0, true);
        leftSpeaker.Play();
        rightSpeaker.Play();
    }
    
    public void StopPlaying() {
        animator.StopPlaying();
    }

    public void SetRecord(Record record) {
        record.MoveToTransform(recordTransform);
        record.StartLoadingAlbum();
        this.record = record;
    }

    public override void Interact() {
        if (Input.GetButton("Action (Primary)")) {
            animator.StartPlaying();
        }
    }

    public override void StartInteracting() {
        cam.positionTweener.AddMove(cam.GetModifiedDefaultPosition(transform.position, false, false, true), animProps.interactionTransitionLength);

        cam.rotationTweener.ClearMoves();
        cam.rotationTweener.AddMove(cam.GetModifiedDefaultRotation(new Vector3(49, 90, 0), true, true, false), animProps.interactionTransitionLength);

        cam.fovTweener.ClearMoves();
        cam.fovTweener.AddMove(30, animProps.interactionTransitionLength);
    }

    public override void StopInteracting() {
    }
}
