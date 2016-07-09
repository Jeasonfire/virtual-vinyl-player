using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {
    public Activatable activatable;
    public TransformManager buttonBaseTransformManager;

    private bool activated = false;

    public void Toggle() {
        if (buttonBaseTransformManager.InProgress()) {
            return;
        }
        activated = !activated;
        if (activated) {
            buttonBaseTransformManager.scaleTweener.AddMove(buttonBaseTransformManager.GetRelativeScale(new Vector3(1, 0.5f, 1)), 0.25f);
            activatable.Activate();
        } else {
            buttonBaseTransformManager.scaleTweener.AddMove(buttonBaseTransformManager.GetRelativeScale(Vector3.one), 0.25f);
            activatable.Deactivate();
        }
    }
}
