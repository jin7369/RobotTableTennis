using UnityEngine;

public class ResetManager : MonoBehaviour
{
    static ResetManager instance = null;
    public GameObject ball;
    public GameObject arm;
    public GameObject targetObj;
    Target target;
    RobotController rc;
    BallScript bs;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        rc = arm.GetComponent<RobotController>();
        bs = ball.GetComponent<BallScript>();
        target = targetObj.GetComponent<Target>();
    }
    public void Reset()
    {
        rc.Reset();
        bs.Reset();
        target.Reset();
    }

    public static ResetManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
}
