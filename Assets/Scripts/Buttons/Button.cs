using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {
    public Activatable activatable;
    public TransformManager buttonBaseTransformManager;
    public bool locked = false;
    public bool activated = false;

    private bool actuallyActivated = false;

    public void Update() {
        if (buttonBaseTransformManager.InProgress()) {
            return;
        }
        if (activated && !actuallyActivated) {
            buttonBaseTransformManager.scaleTweener.AddMove(buttonBaseTransformManager.GetRelativeScale(new Vector3(1, 0.5f, 1)), 0.25f);
            actuallyActivated = activated;
            activatable.Activate();
        }
        if (!activated && actuallyActivated) {
            buttonBaseTransformManager.scaleTweener.AddMove(buttonBaseTransformManager.GetRelativeScale(Vector3.one), 0.25f);
            actuallyActivated = activated;
            activatable.Deactivate();
        }
    }

    public void Toggle() {
        if (buttonBaseTransformManager.InProgress()) {
            return;
        }
        activated = !activated;
    }
}
