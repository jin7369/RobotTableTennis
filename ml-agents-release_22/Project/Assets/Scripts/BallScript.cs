using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public GameObject Agent;
    private bool isTouchable = true;
    private int bounceCount = 0;

    private Vector3 startPosition;
    void Start() {
        startPosition = transform.position;
    }
    public void Reset() {
        transform.position = startPosition;
        bounceCount = 0;
        isTouchable = true;
    }
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Racket") {
            if (isTouchable) {
                Agent.GetComponent<RobotController>().AddReward(1f);
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
        else if (collision.gameObject.tag == "Floor") {
            if (bounceCount == 2) {
                Agent.GetComponent<RobotController>().AddReward(1f);
            }
            else {
                Agent.GetComponent<RobotController>().AddReward(1f);
                Agent.GetComponent<RobotController>().EndEpisode();
            }
        }
        else if (collision.gameObject.tag == "Net") {
            Agent.GetComponent<RobotController>().AddReward(-1f);
            Agent.GetComponent<RobotController>().EndEpisode();
        }
        else if(collision.gameObject.tag == "Table") {
            if(collision.gameObject.GetComponent<Table>().isTouchable) {
                bounceCount += 1;
                Agent.GetComponent<RobotController>().AddReward(1);
            }
            else {
                Agent.GetComponent<RobotController>().AddReward(-1);
                Agent.GetComponent<RobotController>().EndEpisode();
            }
        }


    }
}
