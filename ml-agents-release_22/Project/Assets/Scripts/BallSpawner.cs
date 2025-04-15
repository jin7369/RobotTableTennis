using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballObj;
    Rigidbody ballRigid;

    public GameObject envManagerObj;
    EnvManager envManager;
    public GameObject minPoint;
    public GameObject maxPoint;
    public float minVel;
    public float maxVel;

    public float minAngVel;
    public float maxAngVel;

    Vector3 randomPos;
    Vector3 randomVel;
    Vector3 randomAngVel;

    float minX;
    float minY;
    float minZ;
    float maxX;
    float maxY;
    float maxZ;
    void Start()
    {
        minX = minPoint.transform.position.x;
        minY = minPoint.transform.position.y;
        minZ = minPoint.transform.position.z;

        maxX = maxPoint.transform.position.x;
        maxY = maxPoint.transform.position.y;
        maxZ = maxPoint.transform.position.z;

        ballRigid = ballObj.GetComponent<Rigidbody>();
        envManager = envManagerObj.GetComponent<EnvManager>();
        envManager.reset += Reset;
    }
    
    public void Reset()
    {
        randomPos = new Vector3(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(minY, maxY), UnityEngine.Random.Range(minZ, maxZ));
        randomVel = new Vector3(UnityEngine.Random.Range(minVel, maxVel), UnityEngine.Random.Range(minVel, maxVel), UnityEngine.Random.Range(minVel, maxVel));
        randomAngVel = new Vector3(UnityEngine.Random.Range(minAngVel, maxAngVel), UnityEngine.Random.Range(minAngVel, maxAngVel), UnityEngine.Random.Range(minAngVel, maxAngVel));

        ballObj.transform.position = randomPos;
        ballRigid.velocity = randomVel;
        ballRigid.angularVelocity = randomAngVel;
    }
}
