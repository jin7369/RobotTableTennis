using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public GameObject Agent;
    public bool isTouchable = true;
    private int bounceCount = 0;
    

    public GameObject[] Tables;

    private Vector3 startPosition;
    void Start() {
        startPosition = transform.position;
        Debug.Log(startPosition);
    }
    public void Reset() {
        for (int i = 0; i < Tables.Length; i++) {
            Tables[i].GetComponent<Table>().Reset();
        }
        transform.position = startPosition;
        isTouchable = true;
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        bounceCount = 0;
    }

    void Update () {
        if (transform.position.y < -1.0f) {
            Reset();
            Agent.GetComponent<RobotController>().EndEpisode();
        }
    }
    void OnCollisionEnter(Collision collision) {
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "RacketHead") {
            if (isTouchable) {
                Agent.GetComponent<RobotController>().AddReward(5f);
                Debug.Log("Positive Reward");
                isTouchable = false;
            }
            else {
                Agent.GetComponent<RobotController>().AddReward(-1f);
                Agent.GetComponent<RobotController>().EndEpisode();
            }
        }
        else if (collision.gameObject.tag == "RacketBody") {
            Agent.GetComponent<RobotController>().AddReward(-1f);
            Agent.GetComponent<RobotController>().EndEpisode();
        }
        else if (collision.gameObject.tag == "Net") {
            Agent.GetComponent<RobotController>().AddReward(-1f);
            Agent.GetComponent<RobotController>().EndEpisode();
        }
        else if(collision.gameObject.tag == "Table") {
            if(collision.gameObject.GetComponent<Table>().flag) {
                bounceCount += 1;
                Agent.GetComponent<RobotController>().AddReward(5);
            }
            else {
                Agent.GetComponent<RobotController>().AddReward(-1);
                Agent.GetComponent<RobotController>().EndEpisode();
            }
        }


    }
    void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.tag == "Floor") {
            if (bounceCount == 2) {
                Agent.GetComponent<RobotController>().AddReward(10f);
            }
            else {
                Agent.GetComponent<RobotController>().AddReward(-1f);
                Agent.GetComponent<RobotController>().EndEpisode();
            }
        }
    }
}
