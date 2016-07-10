using UnityEngine;
using System;

[Serializable]
public class RecordPlayerAnimator {
    public RecordPlayer recordPlayer;
    public HingeJointUtils spinnyThing;
    public RPMWatcher spinnyThingWatcher;
    public Animator animator;

    private bool automated = true;
    private bool started = false;
    private bool playing = false;

    public void UpdateAnimations() {
        if (!playing && animator.GetCurrentAnimatorStateInfo(0).IsName("Arm Down")) {
            playing = true;
            recordPlayer.StartPlaying();
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Play")) {
            animator.speed = spinnyThingWatcher.GetSpeedRatio();
        }
    }

    public void StartPlaying() {
        animator.Play("Arm Up");
        spinnyThing.SetMotorForce(75);
    }

    public void Play() {
        if (automated) {
            if (!started) {
                started = true;
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
