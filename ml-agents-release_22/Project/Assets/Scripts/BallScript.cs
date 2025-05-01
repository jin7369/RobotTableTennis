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
        if (collision.gameObject.CompareTag("Table")) {
            tableTennisAgent.Cbt = true;
        }
        else if (collision.gameObject.CompareTag("Paddle")) {
            tableTennisAgent.Cbp = true;
        }
        else {
            tableTennisAgent.EndEpisode();
        }
    }
}
