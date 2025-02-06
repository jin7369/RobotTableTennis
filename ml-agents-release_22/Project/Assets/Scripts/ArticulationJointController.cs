using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.IO;


public class ArticulationJointController : MonoBehaviour
{
    public ArticulationBody articulation;
    public float rotationGoal = 0;


    Quaternion startRotation;

    void Start()
    {
        articulation = GetComponent<ArticulationBody>();
        startRotation = transform.rotation;
        rotationGoal = 0;
    }

    void FixedUpdate() 
    {
        RotateTo(rotationGoal);
    }


    // MOVEMENT HELPERS

    float CurrentPrimaryAxisRotation()
    {
        float currentRotationRads = articulation.jointPosition[0];
        float currentRotation = Mathf.Rad2Deg * currentRotationRads;
        return currentRotation;
    }

    void RotateTo(float primaryAxisRotation)
    {
        var drive = articulation.xDrive;
        drive.target = primaryAxisRotation;
        articulation.xDrive = drive;
    }

    public void Reset() {
        transform.rotation = startRotation;
    }
    public void SetAngle(float rotationGoal) {
        this.rotationGoal = rotationGoal;
    }
    public float GetRotationValue() {
        return articulation.jointPosition[0];
    }



}
