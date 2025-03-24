using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnvManager : MonoBehaviour
{
    public GameObject agentObj;
    public string racketHeadTag;
    public float racketHeadReward;
    TableTennisAgent agent; 
    [System.Serializable]
    public class Target {
        [SerializeField]private GameObject[] targetObjs;
        [SerializeField]private Material activatedTargetMaterial;
        [SerializeField]private float reward;
        [SerializeField]private float negative;
        [SerializeField]private GameObject agentObj;
        [SerializeField]private TableTennisAgent agent;
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
            agent = agentObj.GetComponent<TableTennisAgent>();
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
        public void PlusReward() {
            agent.AddReward(reward);
        }
        public void MinusReward() {
            agent.AddReward(negative);
        }
    }
    public Target[] targets;
    protected int targetCount;
    protected int targetsLen;

    protected virtual void Start()
    {
        agent = agentObj.GetComponent<TableTennisAgent>();
        targetCount = 0;
        targetsLen = targets.Length;
        for (int i = 0; i < targetsLen; i++) {
            targets[i].Initialize();
        }
        targets[0].Activate();
    }
    public virtual void BallCollideWith(GameObject obj) {
        if (obj.CompareTag(racketHeadTag)) {
            agent.AddReward(racketHeadReward);
        }
    }
    protected virtual void Reset()
    {
        if (targetCount > 0) {
            targets[targetCount].DeActivate();
            targetCount = 0;
            targets[targetCount].Activate();
        }
        agent.EndEpisode();
    }
}
