using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class RecordPlayerAnimator {
    public RecordPlayer recordPlayer;
    public HingeJointUtils spinnyThing;
    public RPMWatcher spinnyThingWatcher;
    public TransformManager hand;
    public float recordAngleStart;
    public float recordAngleEnd;
    public float handAngleUp;
    public float handAngleDown;

    private RecordPlayerAnimationProperties animProps;

    void Start() {
        SetHandRot(new Vector3(0, 0, handAngleDown), animProps.handVerticalTransitionLength, false, false, true);
    }

    public void SetAnimProps(RecordPlayerAnimationProperties animProps) {
        this.animProps = animProps;
    }
    
    public void ToggleHand() {
        if (MovingHand()) {
            return;
        } else {
            if (IsHandUp()) {
                SetHandRot(new Vector3(0, 0, handAngleDown), animProps.handVerticalTransitionLength, false, false, true);
            } else {
                SetHandRot(new Vector3(0, 0, handAngleUp), animProps.handVerticalTransitionLength, false, false, true);
            }
        }
    }

    /* Checks */

    public bool MovingHand() {
        return hand.InProgress();
    }

    public bool IsHandUp() {
        return GetHandRot().z == -handAngleUp;
    }

    public bool IsHandDown() {
        return GetHandRot().z == -handAngleDown;
    }

    public bool CanStartPlaying() {
        return GetHandRot().y >= recordAngleStart && GetHandRot().y < recordAngleEnd;
    }

    public bool ShouldStopPlaying() {
        return GetHandRot().y >= recordAngleEnd;
    }

    public bool IsPlaying() {
        return spinnyThingWatcher.GetSpeedRatio() > 0;
    }

    /* Hand manipulation */

    private void SetHandPos(Vector3 pos, float length, bool applyX = true, bool applyY = true, bool applyZ = true) {
        hand.positionTweener.AddMoveXYZ(hand.GetRelativePosition(pos), length, applyX, applyY, applyZ);
    }

    private void SetHandRot(Vector3 rot, float length, bool applyX = true, bool applyY = true, bool applyZ = true) {
        hand.rotationTweener.AddMoveXYZ(hand.GetRelativeRotation(rot) * -1, length, applyX, applyY, applyZ);
    }

    private Vector3 GetHandPos() {
        return hand.positionTweener.GetPositionAtTime(Time.time);
    }

    private Vector3 GetHandRot() {
        return hand.rotationTweener.GetPositionAtTime(Time.time);
    }
}
