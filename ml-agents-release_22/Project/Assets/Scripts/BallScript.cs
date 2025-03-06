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

    // 공 관련 
    Vector3 startPosition;
    Rigidbody rb;

    public GameObject envManagerObj;
    EnvManager env;

    void Start()
    {
        // 자기 자신 관련
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        rb.velocity = Vector3.zero;
        env = envManagerObj.GetComponent<EnvManager>();
    }
    public void Reset()
    {
        transform.position = Vector3.zero;
        transform.position = startPosition;
        rb.velocity = Vector3.zero;
    }
    void OnCollisionEnter(Collision collision)
    {
        env.BallCollideWith(collision.gameObject);
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
