using System;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    /*
    공이 해야할 일
    자신의 현재 위치, 각속도, 속도를 인지하고 원할 때 반환이 가능해야 한다.

    어떠한 물체와 부딪혔을 때 그것을 전달 해야한다.

    인자
    Env Manager : 
    공의 충돌 상태를 확인하고 에이전트에 보상을 준다

    */

    // 에이전트
    public GameObject agentObj;
    TableTennisAgent agent;


    // 타겟
    [System.Serializable]
    public struct TargetContainer {
        public GameObject[] targetObjs;
        public float reward;
        public bool endEpisode;
        public Material originMaterial;
    }
    public Material targetMaterial;
    public TargetContainer[] targets;
    int count;

    // 공 관련 
    Vector3 startPosition;
    Rigidbody rb;


    void Start()
    {
        // 자기 자신 관련
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        rb.velocity = Vector3.zero;


        // 타겟 관련
        count = -1;

        // 에이전트 관련
        agent = agentObj.GetComponent<TableTennisAgent>();
    }
    public void Reset()
    {
        transform.position = Vector3.zero;
        transform.position = startPosition;
        rb.velocity = Vector3.zero;
        count = -1;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (count < 0) {
            if (collision.gameObject.CompareTag("RacketHead")) {
                agent.AddReward(3.0f);
                count++;
                ActivateTarget(count);
            }
            else {
                agent.AddReward(-10.0f);
                agent.EndEpisode();
            }
        }
        if (count > -1 && count < targets.Length) {
            bool targetTouched = false;
            for (int i = 0; i < targets[count].targetObjs.Length; i++) {
                if (ReferenceEquals(targets[count].targetObjs[i], collision.gameObject)) 
                {
                    targetTouched = true; 
                    break;
                }
            }
            if (targetTouched) {
                agent.AddReward(targets[count].reward);
                if (targets[count].endEpisode) agent.EndEpisode();
                DeactivateTarget(count);
                count++;
                ActivateTarget(count);
            }
            else {
                agent.AddReward(-10.0f);
                agent.EndEpisode();
            }
        }
    }
    void ActivateTarget(int targetNum) {
        for (int i = 0; i < targets[targetNum].targetObjs.Length; i++) {
            targets[targetNum].targetObjs[i].GetComponent<MeshRenderer>().material = targetMaterial;
        }
    }
    void DeactivateTarget(int targetNum) {
        for (int i = 0; i < targets[targetNum].targetObjs.Length; i++) {
            targets[targetNum].targetObjs[i].GetComponent<MeshRenderer>().material = targets[targetNum].originMaterial;
        }
    }
    public List<float> GetState() {
        List<float> state = new List<float>
        {
            transform.position.x,
            transform.position.y,
            transform.position.z,
            rb.velocity.x,
            rb.velocity.y,
            rb.velocity.z,
            rb.angularVelocity.x,
            rb.angularVelocity.y,
            rb.angularVelocity.z,
        };
        return state;
    }
}
