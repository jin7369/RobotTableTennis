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
    public GameObject predictionPoint;
    public GameObject targetObj;
    public GameObject ballSpawnArea;

    RandomTarget target;
    public bool Cbp;
    public bool Cbt;
    public bool Pbn;
    Rigidbody ballRb;
    

    public GameObject robot;

    private StreamWriter writer;
    private string path;



    RobotController robotController;
    public override void Initialize()
    {
        if (Application.isBatchMode)
        {
            string[] args = System.Environment.GetCommandLineArgs();
            path = null;
            foreach (var arg in args)
            {
                if (arg.StartsWith("--log_path="))
                {
                    path = arg.Substring("--log_path=".Length).Trim();
                }
            }

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Error: no --log_path element");
                return;
            }
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm");
                string filename = $"{timestamp}_ball_state.csv";
                string descPath = Path.Combine(path, "reward_descriptrion.txt");
                try
                {
                    using (StreamWriter descWriter = new StreamWriter(descPath, append: false))
                    {

                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Fail to create reward description file {e.Message}");
                }
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, filename);
                writer = new StreamWriter(path, append: false);
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
        target = targetObj.GetComponent<RandomTarget>();
    }

    public override void OnEpisodeBegin()
    {
        robotController.Reset();
        target.Reset();
        resetBallState();
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
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
        // 8차원 
        List<float> robotState = robotController.GetState();
        foreach (float value in robotState)
        {
            sensor.AddObservation(value);
        }
        // 24차원 

        // 총 32차원 
        float reward = 0.0f;
        if (Cbp && !Cbt)
        {
            Vector3 predictedLandingPoint = PredictLandingPoint(ballObj.transform.position, ballRb.velocity);
            predictionPoint.SetActive(true);
            predictionPoint.transform.position = predictedLandingPoint;
            reward = 1 + Mathf.Exp(-4 * (predictedLandingPoint - targetObj.transform.position).sqrMagnitude);
            reward *= Mathf.Exp(-(targetLocalPosition - ballLocalPosition).sqrMagnitude);
            //reward *= Mathf.Exp(-Mathf.Pow(ballLocalPosition.y - targetLocalPosition.y, 2.0f));
            bool cond1 = target.max_x >= predictedLandingPoint.x && target.min_x <= predictedLandingPoint.x;
            bool cond2 = target.max_z >= predictedLandingPoint.z && target.min_z <= predictedLandingPoint.z;
            if (cond1 && cond2)
            {
                Debug.Log("Table On");
                reward *= 1.5f;
            }
        }
        AddReward(reward);
    }

    Vector3 PredictLandingPoint(Vector3 position, Vector3 velocity)
    {
        float g = Mathf.Abs(Physics.gravity.y);
        float vy = velocity.y;
        float y0 = position.y;

        float discriminant = vy * vy + 2 * g * y0;
        if (discriminant < 0)
        {
            return Vector3.zero;
        }

        float t = (vy + Mathf.Sqrt(discriminant)) / g;
        float x = position.x + velocity.x * t;
        float z = position.z + velocity.z * t;
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
        Vector3 ballNewPos = center + new Vector3(x, y, z);
        ballObj.transform.position = ballNewPos;
    }

}
