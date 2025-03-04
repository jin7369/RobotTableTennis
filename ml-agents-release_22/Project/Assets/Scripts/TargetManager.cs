using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [System.Serializable]
    class Target {
        public float reward;
        public bool endEpisode;
        public GameObject targetObj;
    }
    public GameObject[] targetObjs;
    ActivatableObject[] targets;
    int currentTarget;
    void Start()
    {
        currentTarget = 0;
        targets = new ActivatableObject[targetObjs.Length];
        for (int i = 0; i < targetObjs.Length; i++) {
            targets[i] = targetObjs[i].GetComponent<ActivatableObject>();
        }
    }
    public void ActivateNextTarget() {
        targets[currentTarget].Deactivate();
        currentTarget++;
        targets[currentTarget].Activate();
    }
    public void Reset()
    {
        currentTarget = 0;
        targets[0].Activate();
        for (int i = 1; i < targetObjs.Length; i++) {
            targets[i].Deactivate();
        }
    }
}
