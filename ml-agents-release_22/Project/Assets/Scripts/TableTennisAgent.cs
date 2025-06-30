using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using Unity.Collections;
using System.IO;
using System.IO.Pipes;
using System.Runtime.CompilerServices;


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
    public GameObject[] paddleDots;
    public GameObject predCollisionWithPaddleObj;
    public GameObject predLandingObj;
    public GameObject targetObj;
    public GameObject targetMaxHeightObj;
    public GameObject predMaxHeightObj;
    public float targetMaxHeight;
    public MeshRenderer ballSpawnArea;
    public MeshRenderer ballLandingArea;
    public MeshRenderer targetArea;
    public MeshRenderer net;
    public bool Cbp;
    public bool Cbt;
    Rigidbody ballRb;


    public GameObject robot;

    private StreamWriter writer;
    private string path;


    public float reward_before;



    RobotController robotController;
    public override void Initialize()
    {
        reward_before = 0.0f;
        if (Application.isBatchMode)
        {
            path = Application.persistentDataPath;
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm");
                string filename = $"{timestamp}_ball_state.csv";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fullPath = Path.Combine(path, filename);
                writer = new StreamWriter(fullPath, append: false);
                writer.WriteLine("ball_x,ball_z,target_x,target_z,cbp,cbt");

            }
            catch (Exception e)
            {
                Debug.LogError($"Exception raised with creating log files: {e.Message}");
            }

        }
        robotController = robot.GetComponent<RobotController>();
        ballRb = ballObj.GetComponent<Rigidbody>();
        resetBallState();
        //SetTargetMaxHeight();
    }

    public override void OnEpisodeBegin()
    {
        reward_before = 0.0f;
        robotController.Reset();
        resetBallState();
        //SetTargetMaxHeight();
        SetTargetPosition();
        Cbp = false;
        Cbt = false;
        predLandingObj.SetActive(false);
        predMaxHeightObj.SetActive(false);
        //Vector3 targetHeightVec = targetMaxHeightObj.transform.position;
        //targetHeightVec.y = targetMaxHeight;
        //targetMaxHeightObj.transform.position = targetHeightVec;
    }
    public List<float> GetState()
    {
        List<float> stateList = new List<float>();
        Vector3 ballLocalPosition = transform.InverseTransformDirection(transform.position - ballObj.transform.position);
        Vector3 ballLocalVelocity = transform.InverseTransformDirection(ballRb.velocity);
        Vector3 targetLocalPosition = transform.InverseTransformDirection(transform.position - targetObj.transform.position);
        stateList.Add(ballLocalPosition.x);
        stateList.Add(ballLocalPosition.y);
        stateList.Add(ballLocalPosition.z);

        stateList.Add(ballLocalVelocity.x);
        stateList.Add(ballLocalVelocity.y);
        stateList.Add(ballLocalVelocity.z);

        stateList.Add(targetLocalPosition.x);
        stateList.Add(targetLocalPosition.z);
        List<float> robotState = robotController.GetState();
        stateList.AddRange(robotState);
        return stateList;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        List<float> stateList = GetState();
        sensor.AddObservation(stateList);
        float reward = 0.0f;
        if (Cbt && !Cbp)
        {
            if (predictPaddleCollision())
            {
                reward += Mathf.Exp(-4 * (predCollisionWithPaddleObj.transform.position - paddleObj.transform.position).sqrMagnitude);
            }
            AddReward(reward - reward_before);
            reward_before = reward;
        }
        // 총 33차원 

        if (Cbp && !Cbt)
        {
            Vector3 predictedLandingPoint = PredictLandingPoint();
            predLandingObj.SetActive(true);
            predLandingObj.transform.position = predictedLandingPoint;
        
            if (CheckInTargetArea(predLandingObj.transform.position))
            {
                float sqrDistWithTarget = Vector3.SqrMagnitude(predLandingObj.transform.position - targetObj.transform.position);
                reward += 3.0f + Mathf.Exp(-4 * sqrDistWithTarget);
            }
            else
            {
                float sqrDistWithCenter_x = Mathf.Pow(predLandingObj.transform.position.x - targetArea.bounds.center.x, 2.0f);
                float sqrDistWithCenter_z = Mathf.Pow(predLandingObj.transform.position.z - targetArea.bounds.center.z, 2.0f);
                float x_length = targetArea.bounds.size.x;
                float z_length = targetArea.bounds.size.z;
                float ratio_x = x_length / (x_length + z_length);
                float ratio_z = 1 - ratio_x;
                reward += 1.0f;
                reward += Mathf.Exp(-4 * ratio_z * sqrDistWithCenter_x) + Mathf.Exp(-4 * ratio_x * sqrDistWithCenter_z);
            }
            AddReward(reward - reward_before);
            reward_before = reward;
        }
    }
    bool CheckInTargetArea(Vector3 position) {
        Vector3 size = targetArea.bounds.size;
        Vector3 center = targetArea.bounds.center;
        bool cond1 = position.x <= center.x + size.x;
        bool cond2 = position.x >= center.x - size.x;
        bool cond3 = position.z <= center.z + size.z;
        bool cond4 = position.z >= center.z - size.z;
        Debug.Log("In Target Area");

        return cond1 && cond2 && cond3 && cond4;
    }
    float Gaussian(float sigma, float x)
    {
        float g = x / sigma;
        return Mathf.Exp(-1.0f * g * g) / (sigma * sigma);
    }
    float PredictMaxHeight()
    {
        float g = Mathf.Abs(Physics.gravity.y);
        float v_y = ballRb.velocity.y;
        float p_y = ballObj.transform.position.y;
        float predictedMaxHeight = v_y * v_y / (2 * g) + p_y;
        return predictedMaxHeight;
    }
    void SetTargetMaxHeight()
    {
        Vector3 center = net.bounds.center;
        Vector3 size = net.bounds.size;
        targetMaxHeight = center.y + size.y * 0.5f + UnityEngine.Random.Range(0.10f, 1.5f);
    }

    Vector3 PredictLandingPoint()
    {
        float g = Mathf.Abs(Physics.gravity.y);
        float vy = ballRb.velocity.y;
        float y0 = ballObj.transform.position.y;

        float discriminant = vy * vy + 2 * g * y0;
        if (discriminant < 0)
        {
            return Vector3.zero;
        }

        float t = (vy + Mathf.Sqrt(discriminant)) / g;
        float x = ballObj.transform.position.x + ballRb.velocity.x * t;
        float z = ballObj.transform.position.z + ballRb.velocity.z * t;
        float y = ballObj.transform.localScale.y * 0.5f;
        return new Vector3(x, y, z);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        for (int i = 0; i < actions.ContinuousActions.Length; i++)
        {
            var action = Mathf.Clamp(actions.ContinuousActions[i], -1f, 1f);
            robotController.ControlTargetPosition(i, action);
        }
    }
    public void EndEpisodeWithSave()
    {
        Vector3 ballPos = ballObj.transform.position;
        Vector3 targetPos = targetObj.transform.position;
        int cbp_val = Cbp ? 1 : 0;
        int cbt_val = Cbt ? 1 : 0;
        cbt_val = cbp_val * cbt_val;
        string line =
        $"{ballPos.x:F3},{ballPos.z:F3}," +
        $"{targetPos.x:F3},{targetPos.z:F3}," +
        $"{cbp_val},{cbt_val}";

        writer.WriteLine(line);

        EndEpisode();
    }
    void OnApplicationQuit()
    {
        if (writer != null)
        {
            writer.Flush();
            writer.Close();
        }
    }
    void resetBallState()
    {
        Vector3 center = ballSpawnArea.bounds.center;
        Vector3 size = ballSpawnArea.bounds.size;
        float x = UnityEngine.Random.Range(-0.5f, 0.5f) * size.x;
        float y = UnityEngine.Random.Range(-0.5f, 0.5f) * size.y;
        float z = UnityEngine.Random.Range(-0.5f, 0.5f) * size.z;
        float g = Mathf.Abs(Physics.gravity.y);
        Vector3 ballNewPos = center + new Vector3(x, y, z);
        ballObj.transform.position = ballNewPos;
        float max_height = net.bounds.center.y + 0.5f * net.bounds.size.y + UnityEngine.Random.Range(0.03f, 0.5f);
        float v_y = Mathf.Sqrt(2 * g * Mathf.Abs(max_height - y));
        float t = (v_y + Mathf.Sqrt(v_y * v_y + 2 * g * ballNewPos.y)) / g;

        center = ballLandingArea.bounds.center;
        size = ballLandingArea.bounds.size;
        float landing_x = center.x + UnityEngine.Random.Range(-0.5f, 0.5f) * size.x;
        float landing_z = center.z + UnityEngine.Random.Range(-0.5f, 0.5f) * size.z;

        float v_x = (landing_x - ballNewPos.x) / (t + 0.001f);
        float v_z = (landing_z - ballNewPos.z) / (t + 0.001f);
        ballRb.velocity = new Vector3(v_x, v_y, v_z);
    }
    bool predictPaddleCollision()
    {
        Vector3 p1 = paddleDots[0].transform.position;
        Vector3 p2 = paddleDots[1].transform.position;
        Vector3 p3 = paddleDots[2].transform.position;
        Vector3 p4 = paddleDots[3].transform.position;
        Vector3 normal = Vector3.Cross(p2 - p1, p3 - p1).normalized;
        Vector3 p = ballObj.transform.position;
        float g = Mathf.Abs(Physics.gravity.y);
        Vector3 v = ballRb.velocity;

        float a = -0.5f * g * normal.y;
        float b = Vector3.Dot(normal, v);
        float c = Vector3.Dot(normal, p - p4);
        float discriminant = b * b - 4 * a * c;
        if (discriminant < 0)
        {
            return false;
        }
        float t1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
        float t2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
        float t = Mathf.Min(t1, t2);
        if (t < 0) t = Mathf.Max(t1, t2);
        if (t < 0) return false;
        Vector3 predict_pos = p + t * v + new Vector3(0.0f, -0.5f * g * t * t, 0.0f);
        predCollisionWithPaddleObj.transform.position = predict_pos;
        return true;
    }
    void SetTargetPosition()
    {
        Vector3 center = targetArea.bounds.center;
        Vector3 size = targetArea.bounds.size;
        float x = UnityEngine.Random.Range(-0.5f, 0.5f) * size.x;
        float z = UnityEngine.Random.Range(-0.5f, 0.5f) * size.z;
        float y = ballObj.transform.localScale.y * 0.5f;
        targetObj.transform.position = center + new Vector3(x, y, z);
    }


}
