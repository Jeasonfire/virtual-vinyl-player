using UnityEngine;
using System.Collections;
using System;

public class RecordPlayer : Interactible {
    public CameraManager cam;
    public RecordPlayerAnimator animator;
    public Transform recordTransform;
    public AudioSource leftSpeaker;
    public AudioSource rightSpeaker;

    private Record record;

    private void StartPlaying() {
        animator.StartPlaying();
        RecordSide current = record.GetCurrentSide();
        leftSpeaker.clip = current.GetClip(0, false);
        rightSpeaker.clip = current.GetClip(0, true);
        leftSpeaker.Play();
        rightSpeaker.Play();
    }
    
    private void StopPlaying() {
        animator.StopPlaying();
    }

    public void SetRecord(Record record) {
        record.MoveToTransform(recordTransform);
        record.StartLoadingAlbum();
        this.record = record;
    }

    public override void Interact() {
        if (Input.GetButton("Action (Primary)")) {
            StartPlaying();
        }
    }

    public override void StartInteracting() {
        cam.targetPosition = cam.GetDefaultPosition();
        cam.targetPosition.z = transform.position.z;
        cam.targetRotation = cam.GetDefaultRotation();
        cam.targetRotation.y = 90;
        cam.targetRotation.x = 49;
        cam.targetFov = cam.GetDefaultFov();
        cam.targetFov = 30;
    }

    public override void StopInteracting() {
    }
}
