using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class TableTennisAgent : Agent
{
    public GameObject ball;
    public GameObject robot;

    RobotController robotController;
    BallScript ballScript;

    public static TableTennisAgent Instance
    {
        get;
        private set;
    }
    
    public override void Initialize()
    {
        if (Instance == null) {
            Instance = this;
        }
        ballScript = ball.GetComponent<BallScript>();
        robotController = robot.GetComponent<RobotController>();
    }

    public override void OnEpisodeBegin()
    {
        ResetManager.Instance.Reset();
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        List<float> ballState = ballScript.GetState();
        List<float> robotState = robotController.GetState();

        foreach (float value in ballState) {
            sensor.AddObservation(value);
        }
        foreach(float value in robotState) {
            sensor.AddObservation(value);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        for (int i = 0; i < actions.ContinuousActions.Length; i++) {
            var action = 180 * Mathf.Clamp(actions.ContinuousActions[i], -1f, 1f);
            robotController.ControlTargetPosition(i, action);
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetKey(KeyCode.LeftArrow) ? -1f : (Input.GetKey(KeyCode.RightArrow) ? 1f : 0f);
        continuousActions[1] = Input.GetKey(KeyCode.UpArrow) ? 1f : (Input.GetKey(KeyCode.DownArrow) ? -1f : 0f);
    }
}
