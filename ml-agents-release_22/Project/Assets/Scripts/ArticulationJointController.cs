using UnityEngine;



public class ArticulationJointController : MonoBehaviour
{
    public float scaleValue;
    ArticulationBody articulation;
    void Start()
    {
        articulation = GetComponent<ArticulationBody>();
    }

    public void Reset() {
        SetTargetPosition(0f);
        articulation.jointPosition = new ArticulationReducedSpace(0f);
        articulation.jointForce = new ArticulationReducedSpace(0f);
        articulation.jointVelocity = new ArticulationReducedSpace(0f);
    }
    public void SetTargetPosition(float targetPosition) {
        var drive = articulation.xDrive;
        drive.target = targetPosition * scaleValue;
        articulation.xDrive = drive;
    }
    public float[] GetState() {
        var state = new float[3];
        state[0] = articulation.jointPosition[0];
        state[1] = articulation.jointForce[0];
        state[2] = articulation.jointVelocity[0];
        return state;
    }
}
