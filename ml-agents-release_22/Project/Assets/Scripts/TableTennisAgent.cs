using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class TableTennisAgent : Agent
{
    static TableTennisAgent instance =  null;
    public GameObject ball;
    public GameObject robot;

    RobotController robotController;
    BallScript ballScript;

    public static TableTennisAgent Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
    
    public override void Initialize()
    {
        instance = this;
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

        sensor.AddObservation(ballState);
        sensor.AddObservation(robotState);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        for (int i = 0; i < actions.ContinuousActions.Length; i++) {
            var action = 180 * Mathf.Clamp(actions.ContinuousActions[i], -1f, 1f);
            robotController.ControlTargetPosition(i, action);
        }
    }
}
