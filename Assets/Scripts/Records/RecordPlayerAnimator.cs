using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class RecordPlayerAnimator {
    public RecordPlayer recordPlayer;
    public HingeJointUtils spinnyThing;
    public RPMWatcher spinnyThingWatcher;

    void FixedUpdate() {
    }

    public void StartPlaying() {
        spinnyThing.SetMotorForce(50);
    }

    public void StopPlaying() {
        spinnyThing.SetMotorForce(0);
    }

    public bool IsPlaying() {
        return spinnyThingWatcher.GetSpeedRatio() > 0;
    }
}
