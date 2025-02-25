using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class Target : MonoBehaviour
{
    public bool originState;
    public bool isTarget;
    public GameObject nextTargetObj;
    Target nextTarget;
    public Material targetMat;
    public Material nonTargetMat;
    MeshRenderer mesh;
    void Start()
    {
        Debug.Log("Start!");
        originState = isTarget;
        if (nextTargetObj != null) {
            nextTarget = nextTargetObj.GetComponent<Target>();
        }
        mesh = GetComponent<MeshRenderer>();
        SetMaterial();
        SetTag();
    }
    public void Activate() {
        isTarget = true;
        SetMaterial();
        SetTag();
    }
    void SetMaterial() {
        mesh.material = isTarget ? (targetMat) : (nonTargetMat);
    }
    void SetTag() {
        gameObject.tag = isTarget ? ("Target") : ("Untagged");
    }
    public void CollisionCheck() {
        if (isTarget) {
            isTarget = false;
            SetTag();
            SetMaterial();
            if(nextTargetObj != null) {
                nextTarget.Activate();
            }
            else {
                TableTennisAgent.Instance.AddReward(20.0f);
                TableTennisAgent.Instance.EndEpisode();
            }
        }
    }
    public void Reset()
    {
        isTarget = originState;
        SetMaterial();
        SetTag();
        if (nextTargetObj != null) {
            nextTarget.Reset();
        }
    }
}
