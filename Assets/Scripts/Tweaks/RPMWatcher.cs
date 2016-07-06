using UnityEngine;
using System.Collections;

public class RPMWatcher : MonoBehaviour {
    public Rigidbody body;
    public float RPM = 0;
    public float targetSpeed = 33;

    private float lastRotation;

    void Start () {
        lastRotation = body.rotation.eulerAngles.y;
    }

	void FixedUpdate () {
        float rotation = body.rotation.eulerAngles.y;
        float deltaRotation = rotation - lastRotation;
        RPM = deltaRotation / Time.fixedDeltaTime / 6f;
        lastRotation = rotation;
	}

    public float GetSpeedRatio() {
        return RPM / targetSpeed;
    }
}
