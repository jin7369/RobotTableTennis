using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RobotController : MonoBehaviour
{
    // Constants
    public float damping;
    public float stiffness;


    [System.Serializable]
    public struct Joint
    {
        public string inputAxis;
        public GameObject robotPart;
    }
    public Joint[] joints;
    ArticulationJointController[] jointControllers;
    void Start() {
        jointControllers = new ArticulationJointController[joints.Length];
        for (int i = 0; i < joints.Length; i++) {
            jointControllers[i] = joints[i].robotPart.GetComponent<ArticulationJointController>();
        }
        SetStiffness(stiffness);
        SetDamping(damping);
    }
    public void Reset() {
        for (int i = 0; i < joints.Length; i++) {
            jointControllers[i].Reset();
        }
    }

    public void ControlTargetPosition(int jointNum, float targetPosition) {
        jointControllers[jointNum].SetTargetPosition(targetPosition);
    }
    public void SetTargetVelocity(int jointNum, float targetVelocity) {
        jointControllers[jointNum].SetTargetVelocity(targetVelocity);
    }
    public void SetStiffness(float stiffness) {
        this.stiffness = stiffness;
        for (int i = 0; i < joints.Length; i++) {
            jointControllers[i].SetStiffness(stiffness);
        }
    }
    public void SetDamping(float damping) {
        this.damping = damping;
        for(int i = 0; i < joints.Length; i++) {
            jointControllers[i].SetDamping(damping);
        }
    }

    public List<float> GetState() {
        List<float> state = new List<float>();
        for (int i = 0; i < jointControllers.Length; i++) {
            state.AddRange(jointControllers[i].GetState());
        }
        return state;
    }
}
