using UnityEngine;
using UnityEngine.UI;
public class RobotController : MonoBehaviour
{
    [System.Serializable]
    public struct Joint
    {
        public string inputAxis;
        public GameObject robotPart;
    }
    public Joint[] joints;
    ArticulationJointController[] jointControllers;
    public GameObject jointSliderObject;
    public GameObject angleSliderObject;
    private Slider jointSlider;
    private Slider angleSlider;


    void Start() {
        jointControllers = new ArticulationJointController[joints.Length];
        for (int i = 0; i < joints.Length; i++) {
            jointControllers[i] = joints[i].robotPart.GetComponent<ArticulationJointController>();
        }
        jointSlider = jointSliderObject.GetComponent<Slider>();
        angleSlider = angleSliderObject.GetComponent<Slider>();
        jointSlider.maxValue = joints.Length - 1;
    }
    public void Reset() {
        for (int i = 0; i < joints.Length; i++) {
            jointControllers[i].Reset();
        }
        angleSlider.value = 0;
        jointSlider.value = 0;
    }

    public void Control(int jointNum, float angle) {
        jointControllers[jointNum].SetAngle(angle);
    }
    public void Renew() {
        Control((int)jointSlider.value, angleSlider.value);
    }
}
