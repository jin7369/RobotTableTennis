using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class ActivatableObject : MonoBehaviour
{
    public bool isActivated;
    public Material ActiveMat;
    public Material DeActiveMat;
    public String ActiveTag;
    public String DeActiveTag;
    MeshRenderer mesh;
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        SetMaterial();
        SetTag();
    }
    public void Activate() {
        isActivated = true;
        SetMaterial();
        SetTag();
    }
    void SetMaterial() {
        mesh.material = isActivated ? ActiveMat : DeActiveMat;
    }
    void SetTag() {
        gameObject.tag = isActivated ? ActiveTag : DeActiveTag;
    }
    public void Deactivate() {
        isActivated = false;
        SetMaterial();
        SetTag();
    }
    public void Reset()
    {
        Deactivate();
    }
}
