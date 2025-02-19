using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class BallScript : MonoBehaviour
{

    // Start is called before the first frame update
    Vector3 startPosition;
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        rb.velocity = Vector3.zero;
        Debug.Log(GetState().Count);
    }
    public void Reset()
    {
        transform.position = Vector3.zero;
        transform.position = startPosition;
        rb.velocity = Vector3.zero;
    }
    public void Push() {
        rb.AddForce(Vector3.forward);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("RacketHead")) {
            if (TableTennisAgent.Instance != null) {
                TableTennisAgent.Instance.AddReward(3.0f);
            }
        }
        else if (collision.gameObject.CompareTag("Goal")) {
            if (TableTennisAgent.Instance != null) {
                TableTennisAgent.Instance.AddReward(20.0f);
                TableTennisAgent.Instance.EndEpisode();
            }
        }
        else {
            if (TableTennisAgent.Instance != null) {
                TableTennisAgent.Instance.AddReward(-10.0f);
                TableTennisAgent.Instance.EndEpisode();
            }
        }
    }
    public List<float> GetState() {
        List<float> state = new List<float>
        {
            transform.position.x,
            transform.position.y,
            transform.position.z,
            rb.velocity.x,
            rb.velocity.y,
            rb.velocity.z,
            rb.angularVelocity.x,
            rb.angularVelocity.y,
            rb.angularVelocity.z,
        };
        return state;
    }

    void Update()
    {
        if (Vector3.Distance(startPosition, transform.position) > 10) {
            if (TableTennisAgent.Instance != null) {
                TableTennisAgent.Instance.AddReward(-10.0f);
                TableTennisAgent.Instance.EndEpisode();
            }
        }
    }
}
