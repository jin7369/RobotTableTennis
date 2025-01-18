using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class RotatePart : MonoBehaviour
{
    private ArticulationBody ab;
    private ArticulationDrive drive;

    public void Start() {
        ab = GetComponent<ArticulationBody>();
        drive = ab.xDrive;
        drive.stiffness = 500f;
        drive.damping = 50f;
        drive.forceLimit = 100f;
        float degree = Mathf.Rad2Deg * ab.jointPosition[0];
        drive.target = degree + 10;
        ab.xDrive = drive;
    }
    float GetDegree() {
        return Mathf.Rad2Deg * ab.jointPosition[0];
    }

    public void FixedUpdate() {
        drive = ab.xDrive;
        drive.target = GetDegree() + 10f;
        ab.xDrive = drive;
    }
}
