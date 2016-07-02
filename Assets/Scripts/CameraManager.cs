using UnityEngine;

public class CameraManager : MonoBehaviour {
    public Vector3 targetPosition;
    public Vector3 targetRotation;
    public float movementSpeed = 6;
    public float turningSpeed = 180;

    void Start () {
        targetPosition = transform.position;
        targetRotation = transform.eulerAngles;
    }

	void FixedUpdate () {
        Vector3 deltaPosition = targetPosition - transform.position;
        if (deltaPosition.magnitude > 0.001) {
            transform.position += deltaPosition * movementSpeed * Time.fixedDeltaTime;
        } else {
            transform.position = targetPosition;
        }
        Vector3 deltaAngle = targetRotation - transform.eulerAngles;
        if (deltaAngle.magnitude > 5) {
            transform.Rotate(deltaAngle * turningSpeed * Time.fixedDeltaTime);
        } else {
            transform.eulerAngles = targetRotation;
        }
	}
}
