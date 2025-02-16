using System.Collections.Generic;
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
    void OnTriggerEnter(Collider other)
    {
        if (TableTennisAgent.Instance != null) {
            if (other.CompareTag("Goal")) {
                TableTennisAgent.Instance.AddReward(10.0f);
            }
            else {
                TableTennisAgent.Instance.AddReward(-5.0f);
            }
        }
        ResetManager.Instance.Reset(); 
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("RacketHead")) {
            if (TableTennisAgent.Instance != null) {
                TableTennisAgent.Instance.AddReward(5.0f);
            }
        }
        else {
            if (TableTennisAgent.Instance != null) {
                TableTennisAgent.Instance.AddReward(-10.0f);
            }
            ResetManager.Instance.Reset();
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
            rb.angularVelocity.z
        };
        return state;
    }
}
