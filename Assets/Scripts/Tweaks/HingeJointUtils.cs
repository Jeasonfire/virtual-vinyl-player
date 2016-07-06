using UnityEngine;
using System.Collections;

public class HingeJointUtils : MonoBehaviour {
    public HingeJoint joint;
    
    public void SetSpringTarget(float target) {
        JointSpring spring = joint.spring;
        spring.targetPosition = target;
        joint.spring = spring;
    }

    public void SetMotorForce(float force) {
        JointMotor motor = joint.motor;
        motor.force = force;
        joint.motor = motor;
    }
}
