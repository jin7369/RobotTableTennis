using System.Collections;
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
        ResetManager.Instance.Reset();   
    }
}
