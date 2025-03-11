using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VREnvManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ballObj;
    Vector3 ballStartPosition;
    Rigidbody ballRb;
    void Start()
    {
        ballStartPosition = ballObj.transform.position;
        ballRb = ballObj.GetComponent<Rigidbody>();
        
    }
    // Update is called once per frame
    void Update()
    {
        if (ballObj.transform.position.y < 0) {
            ballObj.transform.position = ballStartPosition;
            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
        }
    }
}
