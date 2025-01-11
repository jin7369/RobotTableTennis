using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float targetFrameRate = 60;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = (int)targetFrameRate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
