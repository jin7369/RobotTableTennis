using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class PaddlePredictor : MonoBehaviour
{
    public GameObject paddleObj;
    ArticulationBody paddleArticulation;
    // Start is called before the first frame update
    void Start()
    {
        paddleArticulation = paddleObj.GetComponent<ArticulationBody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = paddleObj.transform.position + 5.0f * Time.deltaTime * paddleArticulation.velocity;
    }
}
