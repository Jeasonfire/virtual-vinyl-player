using UnityEngine;
using System.Collections;

public class FireLight : MonoBehaviour {
    public Light fire;
    public float angularVariance = 1f;
    public float intensityVariance = 1f;
    public float varianceDelay = 0.1f;

    private Vector3 defaultAngle;
    private float defaultIntensity;
    private float lastUpdateTime;

    void Start () {
        defaultAngle = transform.eulerAngles;
        defaultIntensity = fire.intensity;
        lastUpdateTime = Time.time;
    }

    void Update () {
        if (Time.time - lastUpdateTime > varianceDelay) {
            fire.transform.eulerAngles = defaultAngle + new Vector3(Random.Range(-1, 1f), Random.Range(-1f, 1f), 0) * angularVariance;
            fire.intensity = defaultIntensity + Random.Range(-1f, 1f) * intensityVariance;
            lastUpdateTime = Time.time;
        }
    }
}
