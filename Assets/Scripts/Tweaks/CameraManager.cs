using UnityEngine;

public class CameraManager : TransformManager {
    public FloatTweener fovTweener;
    public float defaultFov;

    private Camera _camera;

    void Start () {
        _camera = GetComponent<Camera>();
        defaultFov = _camera.fieldOfView;
        fovTweener = new FloatTweener(defaultFov);
        InitializeTweeners();
    }

	void Update() {
        _camera.fieldOfView = fovTweener.GetPositionAtTime(Time.time);
        UpdateTweeners();
    }
}
