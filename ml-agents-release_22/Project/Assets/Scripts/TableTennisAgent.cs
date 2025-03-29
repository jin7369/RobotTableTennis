using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

public class TableTennisAgent : Agent
{
    
    /*A
    에이전트가 해야할 일

    공과 로봇으로 부터 각 상태를 알아내야 한다.

    정책으로 인해 결정된 판단으로 로봇을 제어해야 한다.

    - 인자 
    공
    로봇
    
    */
    // 공 관련
    public GameObject ballObj;
    Rigidbody ballRb;
    Vector3 ballStartPosition;

    public GameObject robot;
    public GameObject[] points;
    public float sumRewardPos = 0;
    public float sumRewardVel = 0;
    RobotController robotController;
    public override void Initialize()
    {
        robotController = robot.GetComponent<RobotController>();
        ballRb = ballObj.GetComponent<Rigidbody>();
        ballStartPosition = ballObj.transform.position;
    }

    public override void OnEpisodeBegin()
    {
        sumRewardPos = 0;
        sumRewardVel = 0;
        robotController.Reset();
        ballObj.transform.position = ballStartPosition;
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        
        Vector3 ballLocalPosition = transform.InverseTransformVector(transform.position - ballObj.transform.position);
        Vector3 ballLocalVelocity = transform.InverseTransformDirection(ballRb.velocity);
        Vector3 ballLocalAngularVelocity = transform.InverseTransformDirection(ballRb.angularVelocity);
        sensor.AddObservation(ballLocalPosition);
        sensor.AddObservation(ballLocalVelocity);
        sensor.AddObservation(ballLocalAngularVelocity);
        // 9차원 
        List<float> robotState = robotController.GetState();
        foreach(float value in robotState) {
            sensor.AddObservation(value);
        }
        // 24차원 

        // 총 33차원
        if (Vector3.Magnitude(ballLocalPosition) > 10.0f) {
            EndEpisode();
        }
        

        float max_z = points[0].transform.position.z;
        float min_z = points[0].transform.position.z;
        float reward;
        foreach(var point in points) {
            float z = point.transform.position.z;
            max_z = (z > max_z) ? z : max_z;
            min_z = (z < max_z) ? z : min_z;
        }
        if (ballObj.transform.position.z <= max_z && ballObj.transform.position.z >= min_z) {
            reward = 1/(100.0f+ Math.Abs((transform.position.x - ballObj.transform.position.x) / 3) * 200);
            sumRewardPos += reward;
            AddReward(reward);
        }
        else {
            reward = -1/(100.0f+ Math.Abs(transform.position.x - ballObj.transform.position.x / 3) * 200);
            sumRewardPos += reward;
            AddReward(reward);
        }
        if (ballLocalVelocity.x >= 0) {
            reward = 1/(50.0f+ Math.Abs(transform.position.x - ballObj.transform.position.x / 3) * 10);
            sumRewardVel += reward;
            AddReward(reward);
        }
        else {
            reward = -1/(50.0f+ Math.Abs(transform.position.x - ballObj.transform.position.x / 3) * 10);
            sumRewardVel += reward;
            AddReward(reward);
        }
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        for (int i = 0; i < actions.ContinuousActions.Length; i++) {
            var action = Mathf.Clamp(actions.ContinuousActions[i], -1f, 1f);
            robotController.ControlTargetPosition(i, action);
        }
    }
}
