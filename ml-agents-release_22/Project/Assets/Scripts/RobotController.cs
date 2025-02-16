using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RobotController : MonoBehaviour
{
    // UI Controller
    public GameObject targetPositionController;
    public GameObject jointSelector;
    public GameObject targetVelocityController;

    Slider targetPositionSlider;
    Slider jointSelectSlider;
    Slider targetVelocitySlider;

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
        if (targetPositionController != null) {
            targetPositionSlider = targetPositionController.GetComponent<Slider>();
        }
        if (jointSelector != null) {
            jointSelectSlider = jointSelector.GetComponent<Slider>();
            jointSelectSlider.maxValue = joints.Length - 1;
        }
        if (targetVelocityController != null) {
            targetVelocitySlider = targetVelocityController.GetComponent<Slider>();
        }


        jointControllers = new ArticulationJointController[joints.Length];
        for (int i = 0; i < joints.Length; i++) {
            jointControllers[i] = joints[i].robotPart.GetComponent<ArticulationJointController>();
        }
        Debug.Log(GetState().Count);
    }
    public void Reset() {
        for (int i = 0; i < joints.Length; i++) {
            jointControllers[i].Reset();
        }
        if (targetPositionSlider != null) {
            targetPositionSlider.value = 0;
        }
        if (targetVelocitySlider != null) {
            targetVelocitySlider.value = 0;
        }
        if (jointSelectSlider != null) {
            jointSelectSlider.value = 0;
        }
    }

    public void ControlTargetPosition(int jointNum, float targetPosition) {
        jointControllers[jointNum].SetTargetPosition(targetPosition);
    }
    public void ControlTargetVelocity(int jointNum, float targetVelocity) {
        jointControllers[jointNum].SetTargetVelocity(targetVelocity);
    }
    public void ControlStiffness(float stiffness) {
        this.stiffness = stiffness;
        for (int i = 0; i < joints.Length; i++) {
            jointControllers[i].SetStiffness(stiffness);
        }
    }
    public void ControlDamping(float damping) {
        this.damping = damping;
        for(int i = 0; i < joints.Length; i++) {
            jointControllers[i].SetDamping(damping);
        }
    }

    public void Renew() {
        int selectedJoint = 0;
        if (jointSelectSlider != null) {
            selectedJoint = (int)jointSelectSlider.value;
        }
        float targetPosition = 0;
        if (targetPositionSlider != null) {
            targetPosition = targetPositionSlider.value;
        }
        float targetVelocity = 0;
        if (targetVelocitySlider != null) {
            targetVelocity = targetVelocitySlider.value;
        }
        
        ControlTargetPosition(selectedJoint, targetPosition);
        ControlTargetVelocity(selectedJoint, targetVelocity);
        ControlStiffness(stiffness);
        ControlDamping(damping);
    }

    public List<float> GetState() {
        List<float> state = new List<float>();
        for (int i = 0; i < jointControllers.Length; i++) {
            state.AddRange(jointControllers[i].GetState());
        }
        return state;
    }
}
