using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class TableTennisEnv : MonoBehaviour
{
    // 에이전트 구성 요소 정의
    [System.Serializable]
    public struct AgentElement
    {
        public TableTennisAgent agent;
        public GameObject paddleObj;
        public GameObject targetObj;
        public GameObject predCollisionWithPaddleObj;
        public GameObject[] oppositeTableObjs;
        public MeshRenderer targetAreaMesh;
        public MeshRenderer ballLandingAreaMesh;
        public MeshRenderer receivingAreaMesh;
        public bool ballHasCollidedWithPaddle;
        public bool ballHasCollidedWithTable;
        float rewardBefore;
    }
    public AgentElement[] agentElements;

    // 공통 구성 요소 정의
    public GameObject ballObj;
    Rigidbody ballRb;
    public MeshRenderer ballSpawnAreaMesh;
    public MeshRenderer netMesh;
    public GameObject predLandingObj;

    // 
    int ballTargetAgentIndex;
    void Start()
    {
        ballTargetAgentIndex = 0;
        ballRb = ballObj.GetComponent<Rigidbody>();
    }

    void ResetBallState()
    {
        Vector3 center = ballSpawnAreaMesh.bounds.center;
        Vector3 size = ballSpawnAreaMesh.bounds.size;
        float x = Random.Range(-0.5f, 0.5f) * size.x;
        float y = Random.Range(-0.5f, 0.5f) * size.y;
        float z = Random.Range(-0.5f, 0.5f) * size.z;
        float g = Mathf.Abs(Physics.gravity.y);
        Vector3 ballStartPos = center + new Vector3(x, y, z);
        ballObj.transform.position = ballStartPos;
        float max_height = netMesh.bounds.center.y + 0.5f * netMesh.bounds.size.y + Random.Range(0.03f, 0.5f);
        float v_y = Mathf.Sqrt(2 * g * Mathf.Abs(max_height - y));
        float t = (v_y + Mathf.Sqrt(v_y * v_y + 2 * g * ballStartPos.y)) / g;


        Vector3 ballLandingCenter = agentElements[ballTargetAgentIndex].receivingAreaMesh.bounds.center;
        Vector3 ballLandingSize = agentElements[ballTargetAgentIndex].receivingAreaMesh.bounds.size;

        float landing_x = ballLandingCenter.x + Random.Range(-0.5f, 0.5f) * ballLandingSize.x;
        float landing_z = ballLandingCenter.z + Random.Range(-0.5f, 0.5f) * ballLandingSize.z;

        float v_x = (landing_x - ballStartPos.x) / (t + 0.001f);
        float v_z = (landing_z - ballStartPos.z) / (t + 0.001f);
        ballRb.velocity = new Vector3(v_x, v_y, v_z);
        ballTargetAgentIndex = (ballTargetAgentIndex + 1) % agentElements.Length;
    }
    void SetTargetPosition(int agentNum)
    {
        Vector3 center = agentElements[agentNum].targetAreaMesh.bounds.center;
        Vector3 size = agentElements[agentNum].targetAreaMesh.bounds.size;
        float x = Random.Range(-0.5f, 0.5f) * size.x;
        float z = Random.Range(-0.5f, 0.5f) * size.z;
        agentElements[agentNum].targetObj.transform.position = center + new Vector3(x, 0.0f, z);
    }

    bool PredictPaddleCollision(int agentNum)
    {
        AgentElement agentElement = agentElements[agentNum];
        Vector3 normal = agentElement.paddleObj.transform.right;
        Vector3 pointOnPlane = agentElement.paddleObj.transform.position;
        float g = Mathf.Abs(Physics.gravity.y);
        float a = -0.5f * g * normal.y;
        float b = Vector3.Dot(normal, ballRb.velocity);
        float c = Vector3.Dot(normal, ballObj.transform.position - pointOnPlane);
        float discriminant = b * b - 4 * a * c;
        if (discriminant < 0 || a == 0) return false;

        float divisor = 2 * a;
        float t1 = (-b - Mathf.Sqrt(discriminant)) / divisor;
        float t2 = (-b + Mathf.Sqrt(discriminant)) / divisor;
        float t = Mathf.Min(t1, t2);
        if (t < 0) t = Mathf.Max(t1, t2);
        if (t < 0) return false;
        Vector3 pred_pos = ballObj.transform.position;
        pred_pos += t * ballRb.velocity;
        pred_pos += new Vector3(0.0f, -0.5f * g * t * t, 0.0f);
        agentElement.predCollisionWithPaddleObj.SetActive(true);
        agentElement.predCollisionWithPaddleObj.transform.position = pred_pos;
        return true;
    }

    float GetReward(int agentNum)
    {
        AgentElement agentElement = agentElements[agentNum];
        if (!agentElement.ballHasCollidedWithPaddle)
        {
            if (PredictPaddleCollision(agentNum))
            {
                Vector3 predPos = agentElement.predCollisionWithPaddleObj.transform.position;
                Vector3 paddlePos = agentElement.paddleObj.transform.position;
                float sqrDist = (predPos - paddlePos).sqrMagnitude;
                float theta = -4.0f;
                float reward = Mathf.Exp(theta * sqrDist);
            }
        }
        if (agentElement.ballHasCollidedWithPaddle && !agentElement.ballHasCollidedWithTable)
        {

        }
        return 0.0f;
    }
    
}
