using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VREnvManager : EnvManager
{
    // Start is called before the first frame update
    Vector3 ballStartPosition;
    Rigidbody ballRb;
    void Start()
    {
        Application.targetFrameRate = 60;
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
    public override void BallCollideWith(GameObject obj)
    {
        return;
    }
    public override void Reset()
    {
        ballObj.transform.position = ballStartPosition;
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
    }
}
