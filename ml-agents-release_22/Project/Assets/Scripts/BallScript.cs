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
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Table"))
        {
            tableTennisAgent.Cbt = true;
            if (tableTennisAgent.Cbt && tableTennisAgent.Cbp)
            {
                if (Application.isBatchMode)
                {
                    tableTennisAgent.EndEpisodeWithSave();
                }
                else
                {
                    tableTennisAgent.EndEpisode();
                }
            }
        }
        else if (collision.gameObject.CompareTag("Paddle"))
        {
            //tableTennisAgent.AddReward(1.0f);
            tableTennisAgent.Cbp = true;
            tableTennisAgent.Cbt = false;
            //tableTennisAgent.reward_before = 0.0f;
        }
        else
        {
            Debug.Log("Cum Reward: " + tableTennisAgent.GetCumulativeReward());
            if (Application.isBatchMode)
            {
                tableTennisAgent.EndEpisodeWithSave();
            }
            else
            {
                tableTennisAgent.EndEpisode();
            }
        }
    }
}
