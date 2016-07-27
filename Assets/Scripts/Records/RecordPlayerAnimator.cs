using UnityEngine;
using System;
using System.Collections.Generic;

/*
 * WARNING: This file contains absolutely horrible code. Not recommended reading, at all.
 *
 * not that this program is well made anyway but this is even worse
 */

[Serializable]
public class RecordPlayerAnimator {
	public AudioClip crackleSound;
	public AudioSource crackleSource;
	public bool shouldCrackle = true;

    public Button playButton;
    public RecordPlayer recordPlayer;
    public HingeJointUtils spinnyThing;
    public RPMWatcher spinnyThingWatcher;
    public Animator animator;
    public Transform armAxisYTransform;
    public float armStartAngle = 0;
    public float armEndAngle = 0;

    private Queue<string> animations = new Queue<string>();
    private bool animationControlled = false;
    private bool prepared = false;
    private bool rewind = false;
	private bool paused = false;
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

        if (rewind) {
            playbackPosition -= recordPlayer.GetRecord().GetRecordData().GetLength() * Time.deltaTime;
            if (playbackPosition <= 0) {
                playbackPosition = 0;
                rewind = false;
            }
        } else if (animations.Count > 0 && IsAnimatorIdle()) {
            string[] parts = animations.Dequeue().Split(':');
            int index = int.Parse(parts[0]);
            if (index >= 0) {
                animator.Play(parts[1], index);
            } else {
                switch (parts[1]) {
                    case "Pause": recordPlayer.Pause(); break;
                    case "UnPause":
                        UpdateSong();
                        recordPlayer.UnPause();
                        break;
                    case "UnPlay": playButton.activated = false; break;
                    case "SpinStart": spinnyThing.SetMotorForce(50); break;
                    case "SpinStop": spinnyThing.SetMotorForce(0); break;
                    case "Rewind": rewind = true; break;
                    case "Lock": playButton.locked = true; break;
                    case "UnLock": playButton.locked = false; break;
                    case "Prepare": prepared = true; break;
                    case "UnPrepare": prepared = false; break;
					case "Crackle": if (shouldCrackle) crackleSource.PlayOneShot(crackleSound); break;
                }
            }
        }
    }

    private void UpdateSong() {
        if (!recordPlayer.IsPaused()) {
            playbackPosition += Time.deltaTime;
        }
        if (recordPlayer.HasSongsToPlay(playbackPosition)) {
            recordPlayer.PlaySongAt(playbackPosition);
        } else {
            Rewind();
        }
    }

    private void UpdateArm() {
        float recordLength = recordPlayer.GetRecord().GetRecordData().GetLength();
        Vector3 rot = armAxisYTransform.localEulerAngles;
        rot.y = (armEndAngle - armStartAngle) * (playbackPosition / recordLength) + armStartAngle;
        armAxisYTransform.localEulerAngles = rot;
    }

    public void FastForward() {
        if (!animationControlled && prepared) {
            playbackPosition += 5 * 60 * Time.deltaTime;
            if (recordPlayer.HasSongsToPlay(playbackPosition)) {
                recordPlayer.PlaySongAt(playbackPosition, true);
            }
        }
    }

    public void Rewind() {
        if (prepared) {
            EnqueueAnimation("Lock");
			EnqueueAnimation("Pause");
			if (!paused) {
				EnqueueAnimation("Arm Up", 0);
			}
			EnqueueAnimation("SpinStop");
            EnqueueAnimation("Rewind");
            EnqueueAnimation("Unprepare", 0);
            EnqueueAnimation("Arm Down", 0);
            EnqueueAnimation("UnPrepare");
            EnqueueAnimation("UnPlay");
            EnqueueAnimation("UnLock");
        }
    }

    public void Pause() {
        EnqueueAnimation("Lock");
        EnqueueAnimation("Pause");
        EnqueueAnimation("Arm Up", 0);
        EnqueueAnimation("SpinStop");
		EnqueueAnimation("UnLock");
		paused = true;
    }

    public void UnPause() {
        if (prepared) {
            EnqueueAnimation("Lock");
            EnqueueAnimation("SpinStart");
            EnqueueAnimation("Arm Down", 0);
			EnqueueAnimation("Crackle");
            EnqueueAnimation("UnPause");
            EnqueueAnimation("UnLock");
        } else {
            Prepare();
        }
		paused = false;
    }

    public void Prepare() {
        EnqueueAnimation("Lock");
        EnqueueAnimation("SpinStart");
        EnqueueAnimation("Arm Up", 0);
        EnqueueAnimation("Prepare", 0);
        EnqueueAnimation("Arm Down", 0);
        EnqueueAnimation("Prepare");
        EnqueueAnimation("UnPause");
        EnqueueAnimation("UnLock");
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

    public bool IsReceivingRecords() {
        return !animationControlled && !prepared;
    }
}
