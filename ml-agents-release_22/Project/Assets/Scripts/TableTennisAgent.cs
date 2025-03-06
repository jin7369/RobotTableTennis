using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

public class TableTennisAgent : Agent
{
    /*
    에이전트가 해야할 일

    공과 로봇으로 부터 각 상태를 알아내야 한다.

    정책으로 인해 결정된 판단으로 로봇을 제어해야 한다.

    - 인자 
    공
    로봇
    
    */
    public GameObject ball;
    public GameObject robot;

    RobotController robotController;
    BallScript ballScript;
    public override void Initialize()
    {
        ballScript = ball.GetComponent<BallScript>();
        robotController = robot.GetComponent<RobotController>();
    }

    public override void OnEpisodeBegin()
    {
        ballScript.Reset();
        robotController.Reset();
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
            var action = Mathf.Clamp(actions.ContinuousActions[i], -1f, 1f);
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
