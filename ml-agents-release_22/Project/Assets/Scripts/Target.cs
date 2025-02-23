using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class Target : MonoBehaviour
{
    Boolean originState;
    public Boolean isTarget;
    public GameObject nextTargetObj;
    Target nextTarget;
    public Material targetMat;
    public Material nonTargetMat;
    MeshRenderer mesh;
    void Start()
    {
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
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball")) {
            if (isTarget) {
                isTarget = false;
                TableTennisAgent.Instance.AddReward(10.0f);
                if (nextTargetObj != null) {
                    nextTarget.Activate();
                }
                else {
                    TableTennisAgent.Instance.EndEpisode();
                }
            }
            else {
                TableTennisAgent.Instance.AddReward(-10.0f);
                TableTennisAgent.Instance.EndEpisode();
            }
            SetMaterial();
            SetTag();
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
