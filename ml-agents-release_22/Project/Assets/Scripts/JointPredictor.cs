using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointPredictor : MonoBehaviour
{
    [SerializeField]
    private GameObject[] jointObjs;
    private ArticulationBody[] joints;
    void Start()
    {
        joints = new ArticulationBody[jointObjs.Length];
        for (int i = 0; i < jointObjs.Length; i++)
        {
            joints[i] = jointObjs[i].GetComponent<ArticulationBody>();
        }
    }

    // Update is called once per frame

    void FixedUpdate()
    {
        Matrix4x4 global = Matrix4x4.identity;

        for (int i = 0; i < joints.Length; i++)
        {
            ArticulationBody joint = joints[0];
            Transform jointTf = jointObjs[i].transform;
            Transform parentTf = jointTf.parent;

            Vector3 anchorWorldPos = parentTf.TransformPoint(joint.anchorPosition);
            Quaternion anchorWorldRot = parentTf.rotation * joint.anchorRotation;

            Vector3 axis = anchorWorldRot * Vector3.right;
            float q = joint.jointPosition[0];

            Matrix4x4 local;

            if (joint.jointType == ArticulationJointType.PrismaticJoint)
            {
                Vector3 moved = anchorWorldPos + axis * q;
                local = Matrix4x4.TRS(moved - global.MultiplyPoint(Vector3.zero),
                                    Quaternion.identity,
                                    Vector3.one);
            }
            else if (joint.jointType == ArticulationJointType.RevoluteJoint)
            {
                Quaternion rotation = Quaternion.AngleAxis(q * Mathf.Rad2Deg, axis);
                local = Matrix4x4.TRS(anchorWorldPos - global.MultiplyPoint(Vector3.zero),
                                    rotation,
                                    Vector3.one);
            }
            else
            {
                local = Matrix4x4.TRS(anchorWorldPos - global.MultiplyPoint(Vector3.zero),
                Quaternion.identity, Vector3.one);
            }

            global *= local;
        }

        transform.position = global.MultiplyPoint(Vector3.zero);
    }
}
