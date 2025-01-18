using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorquePart : MonoBehaviour
{
    // Start is called before the first frame update

    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void TorqueX(float torque) {
        rb.AddTorque(Vector3.forward * torque);
    }
    public void TorqueY(float torque) {
        rb.AddTorque(Vector3.up * torque);
    }
    public void TorqueZ(float torque) {
        rb.AddTorque(Vector3.right * torque);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TorqueY(3);
    }
}
