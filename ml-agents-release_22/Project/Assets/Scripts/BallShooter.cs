using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShooter : MonoBehaviour
{

    public GameObject ballObj;
    public MeshRenderer netRenderer;
    public MeshRenderer LandingAreaRenderer;

    Rigidbody ballRb;

    Vector3 center;
    Vector3 size;

    Vector3 landingAreaCenter;
    Vector3 landingAreaSize;
    float g;
    // Start is called before the first frame update
    void Start()
    {
        Bounds bounds = GetComponent<MeshRenderer>().bounds;
        Bounds landingAreaBounds = LandingAreaRenderer.bounds;

        center = bounds.center;
        landingAreaCenter = landingAreaBounds.center;

        size = bounds.size;
        landingAreaSize = landingAreaBounds.size;

        g = Mathf.Abs(Physics.gravity.y);
        ballRb = ballObj.GetComponent<Rigidbody>();
    }

    float RandRange(float abs)
    {
        return Random.Range(-abs, abs);
    }

    void Reset()
    {
        Vector3 randVec = new Vector3(RandRange(0.5f),
                                    RandRange(0.5f),
                                    RandRange(0.5f));
        Vector3 ballStartPos = center + Vector3.Scale(randVec, size);
        ballObj.transform.position = ballStartPos;
        float netHeight = netRenderer.bounds.center.y + 0.5f * netRenderer.bounds.size.y;
        float radius = ballObj.transform.localScale.y * 0.5f;
        float heightPadding = Random.Range(0.0f, 0.5f);

        float maxHeight = netHeight + radius + heightPadding;
        float v_y = Mathf.Sqrt(2 * g * Mathf.Abs(maxHeight - ballStartPos.y));
        float t = v_y * Mathf.Sqrt(v_y * v_y + 2 * g * (ballStartPos.y - radius)) / g;

        float landing_x = landingAreaCenter.x + RandRange(0.5f) * landingAreaSize.x;
        float landing_z = landingAreaCenter.z + RandRange(0.5f) * landingAreaSize.z;

        float v_x = (landing_x - ballStartPos.x) / (t + 0.001f);
        float v_z = (landing_x - ballStartPos.z) / (t + 0.001f);

        ballRb.velocity = new Vector3(v_x, v_y, v_z);
    }
}
