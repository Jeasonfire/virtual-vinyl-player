using UnityEngine;

public class TransformManager : MonoBehaviour {
    public VectorTweener positionTweener;
    public VectorTweener rotationTweener;
    private Vector3 defaultPosition;
    private Vector3 defaultRotation;

    void Start() {
        InitializeTweeners();
    }

    protected void InitializeTweeners() {
        defaultPosition = transform.position;
        defaultRotation = transform.eulerAngles;
        positionTweener = new VectorTweener(defaultPosition);
        rotationTweener = new VectorTweener(defaultRotation);
    }
    
    void Update() {
        UpdateTweeners();
    }

    protected void UpdateTweeners() {
        transform.position = positionTweener.GetPositionAtTime(Time.time);
        transform.eulerAngles = rotationTweener.GetPositionAtTime(Time.time);
    }

    public bool InProgress() {
        return positionTweener.InProgress() || rotationTweener.InProgress();
    }

    public Vector3 GetRelativePosition(Vector3 newPosition = new Vector3(), bool applyX = true, bool applyY = true, bool applyZ = true) {
        return GetModifiedDefaultVector(defaultPosition, newPosition, applyX, applyY, applyZ);
    }

    public Vector3 GetRelativeRotation(Vector3 newRotation = new Vector3(), bool applyX = true, bool applyY = true, bool applyZ = true) {
        return GetModifiedDefaultVector(defaultRotation, newRotation, applyX, applyY, applyZ);
    }

    private Vector3 GetModifiedDefaultVector(Vector3 a, Vector3 b, bool applyX, bool applyY, bool applyZ) {
        Vector3 result = a;
        if (applyX)
            result.x = b.x;
        if (applyY)
            result.y = b.y;
        if (applyZ)
            result.z = b.z;
        return result;
    }
}
