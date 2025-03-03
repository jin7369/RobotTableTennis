using UnityEngine;

public class ResetManager : MonoBehaviour
{
    static ResetManager instance = null;
    public GameObject ball;
    public GameObject arm;
    public GameObject targetManagerObj;

    TargetManager targetManager;
    RobotController robotController;
    BallScript ballScript;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        robotController = arm.GetComponent<RobotController>();
        ballScript = ball.GetComponent<BallScript>();
        targetManager = targetManagerObj.GetComponent<TargetManager>();
    }
    public void Reset()
    {
        robotController.Reset();
        ballScript.Reset();
        targetManager.Reset();
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
