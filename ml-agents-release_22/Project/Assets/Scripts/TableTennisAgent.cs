using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using Unity.Collections;


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
    public GameObject paddleObj;
    public GameObject predictionPoint;
    public GameObject predictedHeight;
    public GameObject targetObj;
    public GameObject targetHeightObj;
    RandomTarget target;
    RandomTarget targetHeight;
    public bool Cbp;
    public bool Cbt;
    public bool Pbn;
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
        target = targetObj.GetComponent<RandomTarget>();
        targetHeight = targetHeightObj.GetComponent<RandomTarget>();
    }

    public override void OnEpisodeBegin()
    {
        robotController.Reset();
        target.Reset();
        targetHeight.Reset();
        ballObj.transform.position = ballStartPosition;
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        Cbp = false;
        Cbt = false;
        Pbn = false;
        predictedHeight.SetActive(false);
        predictionPoint.SetActive(false);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        
        
        Vector3 ballLocalPosition = transform.InverseTransformDirection(transform.position - ballObj.transform.position);
        Vector3 ballLocalVelocity = transform.InverseTransformDirection(ballRb.velocity);
        Vector3 targetLocalPosition = transform.InverseTransformDirection(transform.position - targetObj.transform.position);
        //Vector3 ballLocalAngularVelocity = transform.InverseTransformDirection(ballRb.angularVelocity);

        Vector3 targetHeightLocalPosition = transform.InverseTransformDirection(targetHeight.transform.position);

        if ((targetHeightLocalPosition.z > ballLocalPosition.z) && !Pbn) {
            Pbn = true;
        }

        
        sensor.AddObservation(ballLocalPosition);
        sensor.AddObservation(ballLocalVelocity);
        sensor.AddObservation(targetLocalPosition.x);
        sensor.AddObservation(targetLocalPosition.z);
        //sensor.AddObservation(ballLocalAngularVelocity);
        // 9차원 
        List<float> robotState = robotController.GetState();
        foreach(float value in robotState) {
            sensor.AddObservation(value);
        }
        // 24차원 

        // 총 32차원 
        if (Vector3.Magnitude(ballLocalPosition) > 10.0f) {
            EndEpisode();
        }

        

        float r_p = 0.0f;
        float r_b = 0.0f;
        float r_h = 0.0f;
        if (!Cbp) {
            r_p = Mathf.Exp(-4 * (paddleObj.transform.position - ballObj.transform.position).sqrMagnitude); 
        }

        if (!Cbt && Cbp) {
            predictionPoint.SetActive(true);
            Vector3 predictedLandingPoint = PredictLandingPoint(ballObj.transform.position, ballRb.velocity);
            predictionPoint.transform.position = predictedLandingPoint;
            r_b = 1.0f + Mathf.Exp(-4 * (predictedLandingPoint - targetObj.transform.position).sqrMagnitude);
        }

        if (Cbp && !Pbn) {
            predictedHeight.SetActive(true);
            float predictedPassingHeight = PredictedPassingHeight(ballObj.transform.position, ballRb.velocity);
            predictedHeight.transform.position = new Vector3(predictedHeight.transform.position.x, predictedPassingHeight, predictedHeight.transform.position.z);
            r_h = Mathf.Exp(-4 * Mathf.Pow((predictedPassingHeight - targetHeightObj.transform.position.y), 2));
        }
        
        AddReward(0.1f * r_p + 0.3f * r_b + 0.6f * r_h);
    }
    Vector3 PredictLandingPoint(Vector3 position, Vector3 velocity) {
        float g = Mathf.Abs(Physics.gravity.y);
        float vy = velocity.y;
        float y0 = position.y;

        float discriminant = vy * vy + 2 * g * y0;
        if (discriminant < 0) {
            return Vector3.zero;
        }

        float t = (vy + Mathf.Sqrt(discriminant)) / g;
        float x = position.x + velocity.x * t;
        float z = position.z + velocity.z * t;
        return new Vector3(x, 0, z);
    }
    float PredictedPassingHeight(Vector3 position, Vector3 velocity) {
        float vx = velocity.x;
        float x0 = position.x; // x0 + vx * t = targetHeight.transform.position.x;
        
        
        if (vx == 0.0f) {
            return -0.0f;
        }
        float t = (targetHeight.transform.position.x - x0) / vx;
        float g = Mathf.Abs(Physics.gravity.y);
        return -0.5f * g * t * t + position.y + velocity.y * t;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        for (int i = 0; i < actions.ContinuousActions.Length; i++) {
            var action = Mathf.Clamp(actions.ContinuousActions[i], -1f, 1f);
            robotController.ControlTargetPosition(i, action);
        }
    }
}
