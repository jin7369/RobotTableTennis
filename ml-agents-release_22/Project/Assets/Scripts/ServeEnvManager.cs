using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServeEnvManager : EnvManager
{

    public GameObject agentObj;
    TableTennisAgent agent;

    public GameObject[] targets;
    Material[] originMaterials;
    int count = 0;

    public Material targetMaterial;

    void Start()
    {
        agent = agentObj.GetComponent<TableTennisAgent>();
        originMaterials = new Material[targets.Length];
        for (int i = 0; i < targets.Length; i++)  {
            
        }
    }
    public void Reset()
    {
        
    }
    public override void BallCollideWith(GameObject obj) {
        if (obj.CompareTag("RacketHead")) {
            Debug.Log("Racket");
            agent.AddReward(3.0f);
        }
        
    }
}
