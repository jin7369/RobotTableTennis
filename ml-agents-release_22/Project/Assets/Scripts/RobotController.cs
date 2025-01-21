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
            sensor.AddObservation(GetRotationState(robotPart));
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        for (int i = 0; i < joints.Length; i++) {
            GameObject robotPart = joints[i].robotPart;
            var direction = actionBuffers.DiscreteActions[i];
            var speed = Mathf.Clamp(actionBuffers.ContinuousActions[i], 0f, 1000f);
            switch (direction) {
                case 0:
                UpdateRotationState(RotationDirection.Negative, robotPart);
                break;
                case 1:
                UpdateRotationState(RotationDirection.None, robotPart);
                break;
                case 2:
                UpdateRotationState(RotationDirection.Positive, robotPart);
                break;
                default:
                break;
            }
            robotPart.GetComponent<ArticulationJointController>().speed = speed;
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
        continuousActionsOut[0] = 100;
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 1;
    }



    // CONTROL

    public void StopAllJointRotations()
    {
        for (int i = 0; i < joints.Length; i++)
        {
            GameObject robotPart = joints[i].robotPart;
            UpdateRotationState(RotationDirection.None, robotPart);
        }
    }

    public void RotateJoint(int jointIndex, RotationDirection direction)
    {
        StopAllJointRotations();
        Joint joint = joints[jointIndex];
        UpdateRotationState(direction, joint.robotPart);
    }

    // HELPERS

    static void UpdateRotationState(RotationDirection direction, GameObject robotPart)
    {
        ArticulationJointController jointController = robotPart.GetComponent<ArticulationJointController>();
        jointController.rotationState = direction;
    }

    static float GetRotationState(GameObject robotPart) {
        ArticulationBody ab = robotPart.GetComponent<ArticulationBody>();
        return ab.jointPosition[0];
    }
}
