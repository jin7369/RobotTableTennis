using System;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public GameObject envManagerObj;
    EnvManager envManager;
    void Start()
    {
        envManager = envManagerObj.GetComponent<EnvManager>();   
    }
    void OnCollisionEnter(Collision collision) {
        envManager.BallCollideWith(collision.gameObject);
    }
}
