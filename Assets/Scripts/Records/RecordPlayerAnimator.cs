using UnityEngine;
using System.Collections;

public class RecordPlayerAnimator : MonoBehaviour {
    public HingeJointUtils spinnyThing;
    public RPMWatcher spinnyThingWatcher;

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
