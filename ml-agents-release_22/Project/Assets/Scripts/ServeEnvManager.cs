using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServeEnvManager : EnvManager
{

    public GameObject agentObj;
    TableTennisAgent agent;

    public GameObject[] targets;
    MeshRenderer[] meshRenderers;
    Material[] originMaterials;
    int count = 0;

    public Material targetMaterial;

    void Start()
    {
        agent = agentObj.GetComponent<TableTennisAgent>();
        meshRenderers = new MeshRenderer[targets.Length];
        originMaterials = new Material[targets.Length];
        for (int i = 0; i < targets.Length; i++)  {
            meshRenderers[i] = targets[i].GetComponent<MeshRenderer>();
            originMaterials[i] = meshRenderers[i].material;
        }
        ActivateTarget(count);
    }
    public override void Reset()
    {
        for (int i = 1; i < targets.Length; i++) {
            DeactivateTarget(i);
        }
        count = 0;
        ActivateTarget(count);
    }
    void ActivateTarget(int targetNum) 
    {
        meshRenderers[targetNum].material = targetMaterial;
    }
    void DeactivateTarget(int targetNum) 
    {
        meshRenderers[targetNum].material = originMaterials[targetNum];
    }
    public override void BallCollideWith(GameObject obj) 
    {

        if (obj.CompareTag("RacketHead")) {
            
        }
        else if (ReferenceEquals(obj, targets[count])) 
        {
            DeactivateTarget(count++);
            if (count < targets.Length) ActivateTarget(count);
            else 
            {
                Reset();
                agent.AddReward(1.0f);
                agent.EndEpisode();
            }
        }
        else 
        {
            Reset();
            agent.AddReward(-1.0f);
            agent.EndEpisode();
        }
    }
}
