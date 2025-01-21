using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public bool isTouchable;
    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.tag == "Ball") {
            isTouchable = false;
        }
    }
}
