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
        public bool IsTarget(GameObject obj) {
            for (int i = 0; i < len; i++) {
                if (ReferenceEquals(obj, targetObjs[i])) return true;
            }
            return false;
        }
        public float GetReward() {
            return reward;
        }
    }
    public Target[] targets;
    int targetCount;
    int targetsLen;

    void Start()
    {
        agent = agentObj.GetComponent<TableTennisAgent>();
        targetCount = 0;
        targetsLen = targets.Length;
        for (int i = 0; i < targetsLen; i++) {
            targets[i].Initialize();
        }
        targets[0].Activate();
    }
    public void BallCollideWith(GameObject obj) {
        if (obj.CompareTag(racketHeadTag)) {
            agent.AddReward(racketHeadReward);
        }
        else if(targets[targetCount].IsTarget(obj)) {
            targets[targetCount].DeActivate();
            if (targetCount < targetsLen - 1) {
                agent.AddReward(targets[targetCount].GetReward());
                targetCount++;
                targets[targetCount].Activate();
            }
            else {
                agent.AddReward(targets[targetCount].GetReward());
                Reset();
            }
        }
        else {
            agent.AddReward(-10.0f);
            Reset();
        }
    }
    void Reset()
    {
        if (targetCount > 0) {
            targets[targetCount].DeActivate();
            targetCount = 0;
            targets[targetCount].Activate();
        }
        agent.EndEpisode();
    }
}
