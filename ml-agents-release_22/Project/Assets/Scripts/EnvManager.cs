using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnvManager : MonoBehaviour
{
    public static Action<GameObject> ballCollideWith;
    // 공이 어떤 물체와 충돌시 호출
    public GameObject ballObj;
    // 공 오브젝트

    void Start()
    {   
        ballCollideWith += BallCollideWith;
        // 로봇팔 컨트롤러 초기화 
    }
    void Update()
    {
        if (Vector3.Distance(transform.position, ballObj.transform.position) > 10.0f) {
            Reset();
            TableTennisAgent.endEpisode();
        }   
    }
    public abstract void BallCollideWith(GameObject obj);
    public abstract void Reset();
}
