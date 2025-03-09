using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveEnvManager : EnvManager
{
    public enum Players {Player1, Player2}
    public Players whoCanHitTheBall;
    public GameObject agentObj1;
    public GameObject agentObj2;
    public GameObject ServeArea;

    TableTennisAgent agent1;
    TableTennisAgent agent2;
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
        ServeArea.SetActive(false);
        for (int i = 0; i < targets.Length; i++) {
            targets[i].targetMeshRenderers = new MeshRenderer[targets[i].targetObjs.Length];
            targets[i].targetOriginMaterials = new Material[targets[i].targetObjs.Length];
            for (int j = 0; j < targets[i].targetObjs.Length; j++) {
                targets[i].targetMeshRenderers[j] = targets[i].targetObjs[j].GetComponent<MeshRenderer>();
                targets[i].targetOriginMaterials[j] = targets[i].targetMeshRenderers[j].material;
            }
        }
        agent1 = agentObj1.GetComponent<TableTennisAgent>();
        agent2 = agentObj2.GetComponent<TableTennisAgent>();
        count = 0;
        ActivateTargets(count);
    }
    public override void BallCollideWith(GameObject obj) {
        bool collideWithRacketHead1 = obj.CompareTag("RacketHead1");
        bool collideWithRacketHead2 = obj.CompareTag("RacketHead2");
        if (collideWithRacketHead1 && whoCanHitTheBall == Players.Player1) {
            agent1.AddReward(3.0f);
        }
        else if (collideWithRacketHead2 && whoCanHitTheBall == Players.Player2) {
            agent2.AddReward(3.0f);
        }
        else if (collideWithRacketHead1 && whoCanHitTheBall == Players.Player2) {
            agent1.AddReward(-10.0f);
            Reset();
            TableTennisAgent.endEpisode();
        }
        else if (collideWithRacketHead2 && whoCanHitTheBall == Players.Player1) {
            agent2.AddReward(-10.0f);
            Reset();
            TableTennisAgent.endEpisode();
        }
        else if (ReferenceEquals(obj, ServeArea)) {
            agent1.AddReward(20.0f);
            ServeArea.SetActive(false);
        }
        else if (IsTarget(obj)) {
            if (whoCanHitTheBall == Players.Player1) {
                agent1.AddReward(10.0f);
            }
            else {
                agent2.AddReward(10.0f);
            }
            DeactivateTargets(count);
            count++;
            if (count == 2) {
                whoCanHitTheBall = Players.Player2;
                ServeArea.SetActive(true);
            }
            if (count < targets.Length) {
                ActivateTargets(count);
            }
            else {
                Reset();
                TableTennisAgent.endEpisode();
            }
        }
        else {
            if (whoCanHitTheBall == Players.Player1) {
                agent1.AddReward(-10.0f);
            }
            else {
                agent2.AddReward(-10.0f);
            }
            Reset();
            TableTennisAgent.endEpisode();
        }
        
    }
    public override void Reset() {
        ServeArea.SetActive(false);
        if (count < targets.Length) {
            DeactivateTargets(count);
        }
        whoCanHitTheBall = Players.Player1;
        count = 0;
        ActivateTargets(count);
    }
    
}
