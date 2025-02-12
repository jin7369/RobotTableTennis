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
        targetPositionSlider = targetPositionController.GetComponent<Slider>();
        jointSelectSlider = jointSelector.GetComponent<Slider>();
        targetVelocitySlider = targetVelocityController.GetComponent<Slider>();
        jointSelectSlider.maxValue = joints.Length - 1;


        jointControllers = new ArticulationJointController[joints.Length];
        for (int i = 0; i < joints.Length; i++) {
            jointControllers[i] = joints[i].robotPart.GetComponent<ArticulationJointController>();
        }

    }
    public void Reset() {
        for (int i = 0; i < joints.Length; i++) {
            jointControllers[i].Reset();
        }
        targetPositionSlider.value = 0;
        targetVelocitySlider.value = 0;
        jointSelectSlider.value = 0;
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
        int selectedJoint = (int)jointSelectSlider.value;
        float targetPosition = targetPositionSlider.value;
        float targetVelocity = targetVelocitySlider.value;
        
        ControlTargetPosition(selectedJoint, targetPosition);
        ControlTargetVelocity(selectedJoint, targetVelocity);
        ControlStiffness(stiffness);
        ControlDamping(damping);
    }
}
