using UnityEngine;

public class CameraManager : MonoBehaviour {
    public Vector3 targetPosition;
    public Vector3 targetRotation;
    public float targetFov;

    public float movementSpeed = 6;
    public float turningSpeed = 180;
    public float zoomingSpeed = 60;

    private Camera _camera;
    private Vector3 defaultPosition;
    private Vector3 defaultRotation;
    private float defaultFov;

    void Start () {
        _camera = GetComponent<Camera>();

        targetPosition = defaultPosition = transform.position;
        targetRotation = defaultRotation = transform.eulerAngles;
        targetFov = defaultFov = _camera.fieldOfView;
    }

	void FixedUpdate () {
        Vector3 deltaPosition = targetPosition - transform.position;
        if (deltaPosition.magnitude > 0.001) {
            transform.position += deltaPosition * movementSpeed * Time.fixedDeltaTime;
        } else {
            transform.position = targetPosition;
        }

        Vector3 deltaAngle = targetRotation - transform.eulerAngles;
        if (deltaAngle.magnitude > 1) {
            transform.eulerAngles += deltaAngle * turningSpeed * Time.fixedDeltaTime * 0.05f;
        } else {
            transform.eulerAngles = targetRotation;
        }

        float deltaFov = targetFov - _camera.fieldOfView;
        if (deltaFov > 0.1) {
            _camera.fieldOfView += deltaFov * zoomingSpeed * Time.fixedDeltaTime * 0.05f;
        } else {
            _camera.fieldOfView = targetFov;
        }
	}

    public Vector3 GetDefaultPosition() {
        return defaultPosition;
    }

    public Vector3 GetDefaultRotation() {
        return defaultRotation;
    }

    public float GetDefaultFov() {
        return defaultFov;
    }
}
