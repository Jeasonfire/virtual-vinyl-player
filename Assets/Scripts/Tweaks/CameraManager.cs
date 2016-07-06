using UnityEngine;

public class CameraManager : MonoBehaviour {
    public VectorTweener positionTweener;
    public VectorTweener rotationTweener;
    public FloatTweener fovTweener;
    public Vector3 defaultPosition;
    public Vector3 defaultRotation;
    public float defaultFov;

    private Camera _camera;

    void Start () {
        _camera = GetComponent<Camera>();

        defaultPosition = transform.position;
        defaultRotation = transform.eulerAngles;
        defaultFov = _camera.fieldOfView;

        positionTweener = new VectorTweener(defaultPosition);
        rotationTweener = new VectorTweener(defaultRotation);
        fovTweener = new FloatTweener(defaultFov);
    }

	void Update() {
        transform.position = positionTweener.GetPositionAtTime(Time.time);
        transform.eulerAngles = rotationTweener.GetPositionAtTime(Time.time);
        _camera.fieldOfView = fovTweener.GetPositionAtTime(Time.time);
    }

    public Vector3 GetModifiedDefaultPosition(Vector3 newPosition, bool applyX, bool applyY, bool applyZ) {
        return GetModifiedDefaultVector(defaultPosition, newPosition, applyX, applyY, applyZ);
    }

    public Vector3 GetModifiedDefaultRotation(Vector3 newRotation, bool applyX, bool applyY, bool applyZ) {
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
