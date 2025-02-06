using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem;
public class RobotController : Agent
{
    public GameObject ball;
    Rigidbody m_BallRb;
    EnvironmentParameters m_ResetParams;
    [System.Serializable]
    public struct Joint
    {
        public string inputAxis;
        public GameObject robotPart;
    }
    public Joint[] joints;


    public override void Initialize() {
        m_BallRb = ball.GetComponent<Rigidbody>();
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        //SetResetParameters();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(ball.transform.position - gameObject.transform.position);
        sensor.AddObservation(m_BallRb.velocity);
        sensor.AddObservation(m_BallRb.angularVelocity);
        for (int i = 0; i < joints.Length; i++)
        {
            GameObject robotPart = joints[i].robotPart;
            sensor.AddObservation(robotPart.GetComponent<ArticulationJointController>().GetRotationValue());
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        for (int i = 0; i < joints.Length; i++) {
            GameObject robotPart = joints[i].robotPart;
            var angle =  90.0f *actionBuffers.ContinuousActions[i];
            robotPart.GetComponent<ArticulationJointController>().SetAngle(angle);
        }
    }

    public override void OnEpisodeBegin()
    {
        for (int i = 0; i < joints.Length; i++) {
            GameObject robotPart = joints[i].robotPart;
            robotPart.GetComponent<ArticulationJointController>().Reset();
        }
        ball.GetComponent<BallScript>().Reset();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        if(Input.GetKey(KeyCode.Q)) {
            continuousActionsOut[0] = 1f;
        }
        else if(Input.GetKey(KeyCode.A)) {
            continuousActionsOut[0] = -1f;
        }
    }
}
