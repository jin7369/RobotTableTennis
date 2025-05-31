using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using Unity.Collections;
using System.IO;
using System.IO.Pipes;


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
    public GameObject paddlePredictPos;
    public GameObject predictionPoint;
    public GameObject targetObj;
    public float targetMaxHeight;
    public GameObject ballSpawnArea;
    public GameObject ballLandingArea;
    public GameObject netObj;
    RandomTarget target;
    public bool Cbp;
    public bool Cbt;
    Rigidbody ballRb;


    public GameObject robot;

    private StreamWriter writer;
    private string path;



    RobotController robotController;
    public override void Initialize()
    {
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
        SetTargetMaxHeight();
        target = targetObj.GetComponent<RandomTarget>();
    }

    public override void OnEpisodeBegin()
    {
        robotController.Reset();
        target.Reset();
        resetBallState();
        SetTargetMaxHeight();
        Cbp = false;
        Cbt = false;
        predictionPoint.SetActive(false);
    }

    public override void CollectObservations(VectorSensor sensor)
    {

        Vector3 ballLocalPosition = transform.InverseTransformDirection(transform.position - ballObj.transform.position);
        Vector3 ballLocalVelocity = transform.InverseTransformDirection(ballRb.velocity);
        Vector3 targetLocalPosition = transform.InverseTransformDirection(transform.position - targetObj.transform.position);

        sensor.AddObservation(ballLocalPosition);
        sensor.AddObservation(ballLocalVelocity);
        sensor.AddObservation(targetLocalPosition.x);
        sensor.AddObservation(targetLocalPosition.z);
        sensor.AddObservation(targetMaxHeight);
        // 9차원 
        List<float> robotState = robotController.GetState();
        float reward = 0.0f;
        foreach (float value in robotState)
        {
            sensor.AddObservation(value);
        }
        // 24차원 
        if (Cbt && !Cbp)
        {
            if (predictPaddleCollision())
            {
                reward += Mathf.Exp(-10 * (paddlePredictPos.transform.position - paddleObj.transform.position).sqrMagnitude);
            }
        }
        // 총 33차원 

        if (Cbp && !Cbt)
        {
            Vector3 predictedLandingPoint = PredictLandingPoint();
            predictionPoint.SetActive(true);
            predictionPoint.transform.position = predictedLandingPoint;
            bool cond1 = target.max_x >= predictedLandingPoint.x && target.min_x <= predictedLandingPoint.x;
            bool cond2 = target.max_z >= predictedLandingPoint.z && target.min_z <= predictedLandingPoint.z;
            if (cond1 && cond2)
            {
                reward += Mathf.Exp(-2 * (predictedLandingPoint - targetObj.transform.position).sqrMagnitude);
            }
            else
            {
                reward += Mathf.Exp(-4 * (predictedLandingPoint - targetObj.transform.position).sqrMagnitude);
            }
            float max_height = PredictMaxHeight();
            reward += Mathf.Exp(-10 * Mathf.Pow(max_height - targetMaxHeight, 2));
        }
        Debug.Log(reward);
        AddReward(reward);
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
        MeshRenderer renderer = netObj.GetComponent<MeshRenderer>();
        Vector3 center = renderer.bounds.center;
        Vector3 size = renderer.bounds.size;
        targetMaxHeight = center.y + size.y * 0.5f + UnityEngine.Random.Range(0.03f, 0.5f);
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
        return new Vector3(x, 0, z);
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
        if (ballSpawnArea == null) return;
        MeshRenderer renderer = ballSpawnArea.GetComponent<MeshRenderer>();
        Vector3 center = renderer.bounds.center;
        Vector3 size = renderer.bounds.size;
        float x = UnityEngine.Random.Range(-0.5f, 0.5f) * size.x;
        float y = UnityEngine.Random.Range(-0.5f, 0.5f) * size.y;
        float z = UnityEngine.Random.Range(-0.5f, 0.5f) * size.z;
        float g = Mathf.Abs(Physics.gravity.y);
        Vector3 ballNewPos = center + new Vector3(x, y, z);
        ballObj.transform.position = ballNewPos;
        renderer = netObj.GetComponent<MeshRenderer>();
        float max_height = renderer.bounds.center.y + 0.5f * renderer.bounds.size.y + UnityEngine.Random.Range(0.03f, 0.5f);
        float v_y = Mathf.Sqrt(2 * g * Mathf.Abs(max_height - y));
        float t = (v_y + Mathf.Sqrt(v_y * v_y + 2 * g * ballNewPos.y)) / g;

        renderer = ballLandingArea.GetComponent<MeshRenderer>();
        center = renderer.bounds.center;
        size = renderer.bounds.size;
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
        paddlePredictPos.transform.position = predict_pos;
        return true;
    }

}
