using UnityEngine;

public class TransformManager : MonoBehaviour {
    public VectorTweener positionTweener;
    public VectorTweener rotationTweener;
    public VectorTweener scaleTweener;
    private Vector3 defaultPosition;
    private Vector3 defaultRotation;
    private Vector3 defaultScale;

    void Start() {
        InitializeTweeners();
    }

    protected void InitializeTweeners() {
        defaultPosition = transform.position;
        defaultRotation = transform.eulerAngles;
        defaultScale = transform.localScale;
        positionTweener = new VectorTweener(defaultPosition);
        rotationTweener = new VectorTweener(defaultRotation);
        scaleTweener = new VectorTweener(defaultScale);
    }
    
    void Update() {
        UpdateTweeners();
    }

    protected void UpdateTweeners() {
        transform.position = positionTweener.GetPositionAtTime(Time.time);
        transform.eulerAngles = rotationTweener.GetPositionAtTime(Time.time);
        transform.localScale = scaleTweener.GetPositionAtTime(Time.time);
    }

    public bool InProgress() {
        return positionTweener.InProgress() || rotationTweener.InProgress() || scaleTweener.InProgress();
    }

    public Vector3 GetRelativePosition(Vector3 newPosition = new Vector3(), bool applyX = true, bool applyY = true, bool applyZ = true) {
        return GetModifiedDefaultVector(defaultPosition, newPosition, applyX, applyY, applyZ);
    }

    public Vector3 GetRelativeRotation(Vector3 newRotation = new Vector3(), bool applyX = true, bool applyY = true, bool applyZ = true) {
        return GetModifiedDefaultVector(defaultRotation, newRotation, applyX, applyY, applyZ);
    }

    public Vector3 GetRelativeScale(Vector3 newScale, bool applyX = true, bool applyY = true, bool applyZ = true) {
        Vector3 result = defaultScale;
        if (applyX)
            result.x *= newScale.x;
        if (applyY)
            result.y *= newScale.y;
        if (applyZ)
            result.z *= newScale.z;
        return result;
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
