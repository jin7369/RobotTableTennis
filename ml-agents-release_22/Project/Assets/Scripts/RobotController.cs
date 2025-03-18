using System;
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
    public static Action reset;
    
    void Start() {
        jointControllers = new ArticulationJointController[joints.Length];
        for (int i = 0; i < joints.Length; i++) {
            jointControllers[i] = joints[i].robotPart.GetComponent<ArticulationJointController>();
        }
    }
    public void Reset() {
        for (int i = 0; i < joints.Length; i++) {
            jointControllers[i].Reset();
        }
    }

    public void ControlTargetPosition(int jointNum, float targetPosition) {
        jointControllers[jointNum].SetTargetPosition(targetPosition);
    }

    public List<float> GetState() {
        List<float> state = new List<float>();
        for (int i = 0; i < jointControllers.Length; i++) {
            state.AddRange(jointControllers[i].GetState());
        }
        return state;
    }
}
