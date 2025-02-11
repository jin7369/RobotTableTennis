using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.IO;
using Google.Protobuf.WellKnownTypes;


public class ArticulationJointController : MonoBehaviour
{
    public ArticulationBody articulation;
    void Start()
    {
        articulation = GetComponent<ArticulationBody>();
    }

    public void Reset() {
        SetAngle(0f);
        articulation.jointPosition = new ArticulationReducedSpace(0f);
        articulation.jointForce = new ArticulationReducedSpace(0f);
        articulation.jointVelocity = new ArticulationReducedSpace(0f);
    }
    public void SetAngle(float rotationGoal) {
        var drive = articulation.xDrive;
        drive.target = rotationGoal;
        articulation.xDrive = drive;
    }
    public float[] GetState() {
        var state = new float[3];
        state[0] = articulation.jointPosition[0];
        state[1] = articulation.jointForce[0];
        state[2] = articulation.jointVelocity[0];
        return state;
    }
}
