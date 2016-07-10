using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class RecordPlayerAnimator {
    public Button playButton;
    public RecordPlayer recordPlayer;
    public HingeJointUtils spinnyThing;
    public RPMWatcher spinnyThingWatcher;
    public Animator animator;
    public Transform armAxisYTransform;
    public float armStartAngle = -33;
    public float armEndAngle = -56;

    private bool automated = true;

    private Queue<string> animations = new Queue<string>();
    private int animationQueueLayer = 0;
    private bool started = false;
    private bool playing = false;
    private bool paused = false;
    private bool unpaused = false;
    public float playbackPosition = 0;

    public void UpdateAnimations() {
        if (playing && !paused) {
            float speed = spinnyThingWatcher.GetSpeedRatio();
            animator.speed = speed;
            recordPlayer.SetPlaybackSpeed(speed);
            playbackPosition += speed * Time.deltaTime;

            float recordLength = recordPlayer.GetRecord().GetCurrentSide().GetLength();
            Vector3 rot = armAxisYTransform.localEulerAngles;
            rot.y = (armEndAngle - armStartAngle) * (playbackPosition / recordLength) + armStartAngle;
            armAxisYTransform.localEulerAngles = rot;
        }
        if (!playing && started && animations.Count == 0) {
            playing = true;
            recordPlayer.StartPlaying();
            playButton.locked = false;
        }
        if (paused && unpaused && animations.Count == 0) {
            paused = false;
            unpaused = false;
            recordPlayer.UnPause();
            animationQueueLayer = 0;
            playButton.locked = false;
        }
        if (animations.Count > 0 && IsAnimatorIdle(animationQueueLayer)) {
            animator.Play(animations.Dequeue(), animationQueueLayer);
        }
    }

    public void StartPlaying() {
        animations.Enqueue("Arm Up");
        animations.Enqueue("Prepare");
        animations.Enqueue("Arm Down");
        spinnyThing.SetMotorForce(50);
    }

    public void Pause() {
        recordPlayer.Pause();
        spinnyThing.SetMotorForce(0);
        animator.Play("Arm Up", 1);
    }

    public void UnPause() {
        spinnyThing.SetMotorForce(50);
        animationQueueLayer = 1;
        animations.Enqueue("Arm Down");
        animations.Enqueue("Idle");
    }

    public void Play() {
        if (automated) {
            if (!started) {
                started = true;
                StartPlaying();
                playButton.locked = true;
            }
            if (paused) {
                unpaused = true;
                UnPause();
                playButton.locked = true;
            }
        } else {
            Debug.Log("Manual playing not supported yet!");
        }
    }

    public void Stop() {
        if (automated) {
            if (!paused) {
                paused = true;
                Pause();
            }
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

    private bool IsAnimatorIdle(int layer = 0) {
        return animator.GetCurrentAnimatorStateInfo(layer).IsName("Idle");
    }
}
