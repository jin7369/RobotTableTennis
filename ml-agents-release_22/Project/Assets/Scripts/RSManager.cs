using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSManager : EnvManager
{
    public GameObject agentObj;

    TableTennisAgent agent;
    
    [System.Serializable]
    public struct Targets {
        public GameObject[] targetObjs;
        public MeshRenderer[] targetMeshRenderers;
        public Material[] targetOriginMaterials;

    }
    public Material targetMaterial;
    public Targets[] targets;
    int count = 0;
    bool IsTarget(GameObject obj) {
        for (int i = 0; i < targets[count].targetObjs.Length; i++) {
            if (ReferenceEquals(obj, targets[count].targetObjs[i])) return true;
        }
        return false;
    }
    void ActivateTargets(int targetNum) {
        for (int i = 0; i < targets[targetNum].targetObjs.Length; i++) {
            targets[targetNum].targetMeshRenderers[i].material = targetMaterial;
        }
    }
    void DeactivateTargets(int targetNum) {
        for (int i = 0; i < targets[targetNum].targetObjs.Length; i++) {
            targets[targetNum].targetMeshRenderers[i].material = targets[targetNum].targetOriginMaterials[i];
        }
    }
    void Start()
    {
        for (int i = 0; i < targets.Length; i++) {
            targets[i].targetMeshRenderers = new MeshRenderer[targets[i].targetObjs.Length];
            targets[i].targetOriginMaterials = new Material[targets[i].targetObjs.Length];
            for (int j = 0; j < targets[i].targetObjs.Length; j++) {
                targets[i].targetMeshRenderers[j] = targets[i].targetObjs[j].GetComponent<MeshRenderer>();
                targets[i].targetOriginMaterials[j] = targets[i].targetMeshRenderers[j].material;
            }
        }
        agent = agentObj.GetComponent<TableTennisAgent>();
        count = 0;
        ActivateTargets(count);
    }
    public override void BallCollideWith(GameObject obj) {
        if (obj.CompareTag("RacketHead") && (count == 0 || count == 2)) {
            Debug.Log("Racket Touched!");
            agent.AddReward(3.0f);
        }
        else if (IsTarget(obj)) {
            if (count == targets.Length - 1) {
                Debug.Log("Goal!");
                agent.AddReward(20.0f);
                Reset();
                agent.EndEpisode();
            }
            else {
                Debug.Log("Target Touched!");
                agent.AddReward(10.0f);
                DeactivateTargets(count);
                count++;
                ActivateTargets(count);
            }
        }
        else {
            agent.AddReward(-5.0f);
            Reset();
            agent.EndEpisode();
        }
        
        
    }
    public override void Reset() {
        DeactivateTargets(count);
        count = 0;
        ActivateTargets(count);
    }
    
}
