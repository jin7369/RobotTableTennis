using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public bool isTouchable;
    public bool flag;
    void Start() {
        flag = isTouchable;
    }
    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.tag == "Ball") {
            flag = false;
        }
    }

    public void Reset() {
        flag = isTouchable;
    }
}
