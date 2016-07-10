using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class RecordPlayerAnimator {
    public RecordPlayer recordPlayer;
    public HingeJointUtils spinnyThing;
    public RPMWatcher spinnyThingWatcher;
    public Animator animator;
    
    private Queue<string> queuedAnimations = new Queue<string>();
    private bool automated = true;
    private bool started = false;
    private bool playing = false;

    public void UpdateAnimations() {
        if (queuedAnimations.Count > 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
            animator.PlayInFixedTime(queuedAnimations.Dequeue());
        }
        if (!playing && animator.GetCurrentAnimatorStateInfo(0).IsName("Play")) {
            playing = true;
            spinnyThing.SetMotorForce(50);
            recordPlayer.StartPlaying();
        }
    }

    public void SetArmUp() {
        queuedAnimations.Enqueue("Arm Up");
    }

    public void SetArmDown() {
        queuedAnimations.Enqueue("Arm Down");
    }

    public void MoveToPlayingPosition() {
        queuedAnimations.Enqueue("Prepare Play");
    }

    public void StartPlaying() {
        queuedAnimations.Enqueue("Play");
    }

    public void Play() {
        if (automated) {
            if (!started) {
                started = true;
                SetArmUp();
                MoveToPlayingPosition();
                SetArmDown();
                StartPlaying();
            }
        } else {
            Debug.Log("Manual playing not supported yet!");
        }
    }

    public void Stop() {
        if (automated) {
        } else {
            Debug.Log("Manual stopping not supported yet!");
        }
    }

    public void SetAutomated(bool automated) {
        this.automated = automated;
    }

    public bool IsPlaying() {
        return spinnyThingWatcher.GetSpeedRatio() > 0;
    }
}
