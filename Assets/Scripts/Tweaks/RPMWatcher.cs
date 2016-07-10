using UnityEngine;
using System.Collections;

public class RPMWatcher : MonoBehaviour {
    public Rigidbody body;
    public float RPM = 0;
    public float targetSpeed = 33;

    private int averageSampleIndex = 0;
    private float[] averages = new float[100];
    private float lastRotation;

    void Start () {
        lastRotation = body.rotation.eulerAngles.y;
    }

	void FixedUpdate () {
        float rotation = body.rotation.eulerAngles.y;
        float deltaRotation = rotation - lastRotation;
        float currentRPM = Mathf.Max(0, deltaRotation / Time.fixedDeltaTime / 6f);
        lastRotation = rotation;

        averages[averageSampleIndex++] = currentRPM;
        if (averageSampleIndex >= averages.Length) {
            averageSampleIndex = 0;
        }
        float total = 0;
        foreach (float rpm in averages) {
            total += rpm;
        }
        RPM = Mathf.Ceil(total / averages.Length);
	}

    public float GetSpeedRatio() {
        return RPM / targetSpeed;
    }
}
