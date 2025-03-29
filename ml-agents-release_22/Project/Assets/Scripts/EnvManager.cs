using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class EnvManager : MonoBehaviour
{
    public GameObject[] agentObjs;
    TableTennisAgent[] agents; 
    [System.Serializable]
    public class Target {
        [SerializeField]private GameObject[] targetObjs;
        [SerializeField]private Material activatedTargetMaterial;
        [SerializeField]private float reward;
        [SerializeField]private float negative;
        private MeshRenderer[] meshRenderers;
        private Material[] materials;
        private int len;
        public void Initialize() {
            len = targetObjs.Length;
            meshRenderers = new MeshRenderer[len];
            materials = new Material[len];
            for (int i = 0; i < len; i++) {
                meshRenderers[i] = targetObjs[i].GetComponent<MeshRenderer>();
                materials[i] = meshRenderers[i].material;
            }
        }
        public void Activate() {
            for (int i = 0; i < len; i++) {
                meshRenderers[i].material = activatedTargetMaterial;
            }
        }
        public void DeActivate() {
            for (int i = 0; i < len; i++) {
                meshRenderers[i].material = materials[i];
            }
        }
        public float GetReward() {
            return reward;
        }
        public float GetNegativeReward() {
            return negative;
        }
        public bool IsTarget(GameObject obj) {
            foreach(var targetObj in targetObjs) {
                if (ReferenceEquals(obj, targetObj)) return true;
            }
            return false;
        }
    }
    public Target[] targets;
    protected int targetCount;
    protected int targetsLen;

    protected virtual void Start()
    {
        agents = new TableTennisAgent[agentObjs.Length];
        for (int i = 0; i < agentObjs.Length; i++) {
            agents[i] = agentObjs[i].GetComponent<TableTennisAgent>();
        }
        targetCount = 0;
        targetsLen = targets.Length;
        for (int i = 0; i < targetsLen; i++) {
            targets[i].Initialize();
        }
        targets[0].Activate();
    }
    public virtual void BallCollideWith(GameObject obj) {
        Target target = targets[targetCount];
        if (obj.CompareTag("RacketHead")) {
            if (target.IsTarget(obj)) {
                agents[0].AddReward(target.GetReward());
                targetCount++;
                target.DeActivate();
                targets[targetCount].Activate();
            }
        }
        else {
            if (target.IsTarget(obj)) {
                agents[0].AddReward(target.GetReward());
                if (targetCount < targetsLen - 1) {
                    targetCount++;
                    target.DeActivate();
                    targets[targetCount].Activate();
                }
                else {
                    Reset();
                }
                
            }
            else {
                agents[0].AddReward(target.GetNegativeReward());
                Reset();
            }
        }
    }
    protected virtual void Reset()
    {
        if (targetCount > 0) {
            targets[targetCount].DeActivate();
            targetCount = 0;
            targets[targetCount].Activate();
        }
        foreach (var agent in agents) {
            agent.EndEpisode();
        }
    }
}
