using System;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Extensions.Input;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public GameObject tableTennisAgentObj;
    TableTennisAgent tableTennisAgent;
    void Start()
    {
        tableTennisAgent = tableTennisAgentObj.GetComponent<TableTennisAgent>();
    }
    void Update()
    {
        if (transform.position.y < 0.0f) {
            if (Application.isBatchMode) {
                tableTennisAgent.EndEpisodeWithSave();
            }
            else {
                tableTennisAgent.EndEpisode();
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Table")) {
            //tableTennisAgent.AddReward(1.0f);
            Debug.Log(tableTennisAgent.GetCumulativeReward());
            tableTennisAgent.Cbt = true;
            if (Application.isBatchMode) {
                tableTennisAgent.EndEpisodeWithSave();
            }
            else {
                tableTennisAgent.EndEpisode();
            }

        }
        else if (collision.gameObject.CompareTag("Paddle")) {
            tableTennisAgent.Cbp = true;
        }
        else {
            Debug.Log(tableTennisAgent.GetCumulativeReward());
            if (Application.isBatchMode) {
                tableTennisAgent.EndEpisodeWithSave();
            }
            else {
                tableTennisAgent.EndEpisode();
            }
        }
    }
}
