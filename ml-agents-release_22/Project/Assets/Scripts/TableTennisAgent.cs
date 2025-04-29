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
    public GameObject target;
    Rigidbody ballRb;
    Vector3 ballStartPosition;

    public GameObject robot;
    public GameObject[] points;
    
    

    RobotController robotController;
    public override void Initialize()
    {
        robotController = robot.GetComponent<RobotController>();
        ballRb = ballObj.GetComponent<Rigidbody>();
        ballStartPosition = ballObj.transform.position;
    }

    public override void OnEpisodeBegin()
    {
        robotController.Reset();
        ballObj.transform.position = ballStartPosition;
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        
        Vector3 ballLocalPosition = transform.InverseTransformVector(transform.position - ballObj.transform.position);
        Vector3 ballLocalVelocity = transform.InverseTransformDirection(ballRb.velocity);
        //Vector3 ballLocalAngularVelocity = transform.InverseTransformDirection(ballRb.angularVelocity);
        sensor.AddObservation(ballLocalPosition);
        sensor.AddObservation(ballLocalVelocity);
        //sensor.AddObservation(ballLocalAngularVelocity);
        // 9차원 -> 6차원(각속도 제거)
        List<float> robotState = robotController.GetState();
        foreach(float value in robotState) {
            sensor.AddObservation(value);
        }
        // 24차원 

        // 총 33차원 -> 30차원(각속도 제거)
        if (Vector3.Magnitude(ballLocalPosition) > 10.0f) {
            EndEpisode();
        }
        
        Vector3 acceleration = Physics.gravity;
        double landTime = quadraticFormula(0.5 * acceleration.y, ballLocalVelocity.y, ballStartPosition.y);
        
        
    }
    double quadraticFormula(double a, double b, double c) {
        double ret = -b + Math.Sqrt(Math.Pow(b, 2) - 4 * a * c);
        ret = ret / (2 * a);
        return ret;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        for (int i = 0; i < actions.ContinuousActions.Length; i++) {
            var action = Mathf.Clamp(actions.ContinuousActions[i], -1f, 1f);
            robotController.ControlTargetPosition(i, action);
        }
    }
}
