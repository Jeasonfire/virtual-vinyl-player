using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class RecordPlayerAnimator {
    public RecordPlayer recordPlayer;
    public HingeJointUtils spinnyThing;
    public RPMWatcher spinnyThingWatcher;
    public TransformManager arm;
    public float recordAngleStart;
    public float recordAngleEnd;
    public float handAngleUp;
    public float handAngleDown;

    private RecordPlayerAnimationProperties animProps;
    private bool automated = false;

    void Start() {
        SetHandRot(new Vector3(0, 0, handAngleDown), animProps.handVerticalTransitionLength, false, false, true);
    }

    public void SetAnimProps(RecordPlayerAnimationProperties animProps) {
        this.animProps = animProps;
    }

    public void SetArmUp() {
        SetHandRot(new Vector3(0, 0, handAngleUp), animProps.handVerticalTransitionLength, false, false, true);
    }

    public void SetArmDown() {
        SetHandRot(new Vector3(0, 0, handAngleDown), animProps.handVerticalTransitionLength, false, false, true);
    }

    public void MoveToPlayingPosition() {
        SetHandRot(new Vector3(0, recordAngleStart, 0), animProps.handPlayTransitionLength, false, true, false);
    }

    public void Play() {
        if (automated) {
            SetArmUp();
            MoveToPlayingPosition();
            SetArmDown();
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

    /* Checks */

    public bool MovingArm() {
        return arm.InProgress();
    }

    public bool IsHandUp() {
        return GetArmRot().z == handAngleUp;
    }

    public bool IsHandDown() {
        return GetArmRot().z == handAngleDown;
    }

    public bool CanStartPlaying() {
        return GetArmRot().y >= recordAngleStart && GetArmRot().y < recordAngleEnd;
    }

    public bool ShouldStopPlaying() {
        return GetArmRot().y >= recordAngleEnd;
    }

    public bool IsPlaying() {
        return spinnyThingWatcher.GetSpeedRatio() > 0;
    }

    /* Hand manipulation */

    private void SetHandPos(Vector3 pos, float length, bool applyX = true, bool applyY = true, bool applyZ = true) {
        arm.positionTweener.AddMoveXYZ(pos, length, applyX, applyY, applyZ);
    }

    private void SetHandRot(Vector3 rot, float length, bool applyX = true, bool applyY = true, bool applyZ = true) {
        arm.rotationTweener.AddMoveXYZ(rot * -1, length, applyX, applyY, applyZ);
    }

    private Vector3 GetArmPos() {
        return arm.positionTweener.GetPositionAtTime(Time.time);
    }

    private Vector3 GetArmRot() {
        return arm.rotationTweener.GetPositionAtTime(Time.time) * -1;
    }
}
