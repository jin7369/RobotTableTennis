using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionListener : MonoBehaviour
{
    public GameObject agentObj;
    TableTennisAgent agent;
    public float rewardRacketHead;
    public float rewardTarget;
    public float rewardOther;
    public float rewardGoal;
    void Awake()
    {
        agent = agentObj.GetComponent<TableTennisAgent>();
    }

    public void CollideRacketHead() {
        agent.AddReward(rewardRacketHead);
    }
    public void CollideTarget() {
        agent.AddReward(rewardTarget);
    }
    public void CollideGoal() {
        agent.AddReward(rewardTarget);
        agent.EndEpisode();
    }
    public void CollideOther() {
        agent.AddReward(rewardOther);
        agent.EndEpisode();
    }


}
