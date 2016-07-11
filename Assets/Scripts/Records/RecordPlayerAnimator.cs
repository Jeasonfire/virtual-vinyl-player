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

    private Queue<string> animations = new Queue<string>();
    private bool animationControlled = false;
    private bool prepared = false;
    private bool rewind = false;
    private float playbackPosition = 0;

    public void Update() {
        UpdateAnimations();
        UpdateArm();
        if (!animationControlled && prepared) {
            UpdateSong();
        }
    }

    private void UpdateAnimations() {
        animationControlled = animations.Count != 0 || !IsAnimatorIdle() || rewind;

        if (animations.Count > 0 && IsAnimatorIdle()) {
            string[] parts = animations.Dequeue().Split(':');
            int index = int.Parse(parts[0]);
            if (index >= 0) {
                animator.Play(parts[1], index);
            } else {
                switch (parts[1]) {
                    case "Pause": recordPlayer.Pause(); break;
                    case "UnPause": recordPlayer.UnPause(); break;
                    case "SpinStart": spinnyThing.SetMotorForce(50); break;
                    case "SpinStop": spinnyThing.SetMotorForce(0); break;
                    case "Flip": recordPlayer.GetRecord().Flip(); break;
                    case "Rewind": rewind = true; break;
                    case "Lock": playButton.locked = true; break;
                    case "UnLock": playButton.locked = false; break;
                    case "Prepare": prepared = true; break;
                    case "UnPrepare": prepared = false; break;
                }
            }
        }

        if (rewind) {
            playbackPosition -= recordPlayer.GetRecord().GetCurrentSide().GetLength() * Time.deltaTime;
            if (playbackPosition <= 0) {
                playbackPosition = 0;
                rewind = false;
            }
        }
    }

    private void UpdateSong() {
        playbackPosition += Time.deltaTime;
        if (recordPlayer.HasSongsToPlay(playbackPosition)) {
            recordPlayer.PlaySongAt(playbackPosition);
        } else {
            EnqueueAnimation("Lock");
            EnqueueAnimation("Pause");
            EnqueueAnimation("Arm Up", 1);
            EnqueueAnimation("SpinStop");
            EnqueueAnimation("Rewind");
            EnqueueAnimation("Flip");
            EnqueueAnimation("SpinStart");
            EnqueueAnimation("Arm Down", 1);
            EnqueueAnimation("UnPause");
            EnqueueAnimation("UnLock");
        }
    }

    private void UpdateArm() {
        float recordLength = recordPlayer.GetRecord().GetCurrentSide().GetLength();
        Vector3 rot = armAxisYTransform.localEulerAngles;
        rot.y = (armEndAngle - armStartAngle) * (playbackPosition / recordLength) + armStartAngle;
        armAxisYTransform.localEulerAngles = rot;
    }

    public void FastForward() {
        if (!animationControlled && prepared) {
            playbackPosition += 20 * Time.deltaTime;
            if (recordPlayer.HasSongsToPlay(playbackPosition)) {
                recordPlayer.PlaySongAt(playbackPosition, true);
            }
        }
    }

    public void Pause() {
        EnqueueAnimation("Lock");
        EnqueueAnimation("Pause");
        EnqueueAnimation("Arm Up", 1);
        EnqueueAnimation("SpinStop");
        EnqueueAnimation("UnLock");
    }

    public void UnPause() {
        if (prepared) {
            EnqueueAnimation("Lock");
            EnqueueAnimation("SpinStart");
            EnqueueAnimation("Arm Down", 1);
            EnqueueAnimation("UnPause");
            EnqueueAnimation("UnLock");
        } else {
            EnqueueAnimation("Lock");
            EnqueueAnimation("SpinStart");
            EnqueueAnimation("Arm Up", 0);
            EnqueueAnimation("Prepare", 0);
            EnqueueAnimation("Arm Down", 0);
            EnqueueAnimation("UnPause");
            EnqueueAnimation("UnLock");
            EnqueueAnimation("Prepare");
        }
    }

    public void Play() {
        if (!animationControlled) {
            UnPause();
        }
    }

    public void Stop() {
        if (!animationControlled) {
            Pause();
        }
    }

    private void EnqueueAnimation(string name, int layer = -1) {
        animations.Enqueue(layer + ":" + name);
    }

    private bool IsAnimatorIdle() {
        for (int i = 0; i < animator.layerCount; i++) {
            if (!animator.GetCurrentAnimatorStateInfo(i).IsName("Idle")) {
                return false;
            }
        }
        return true;
    }
}
