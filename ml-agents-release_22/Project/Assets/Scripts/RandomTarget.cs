using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTarget : MonoBehaviour
{
    public GameObject rangePoint1;
    public GameObject rangePoint2;
    float max_x;
    float max_z;
    float min_x;
    float min_z;
    float max_y;
    float min_y;
    void Start()
    {
        max_x = Mathf.Max(rangePoint1.transform.position.x, rangePoint2.transform.position.x);
        min_x = Mathf.Min(rangePoint1.transform.position.x, rangePoint2.transform.position.x);

        max_z = Mathf.Max(rangePoint1.transform.position.z, rangePoint2.transform.position.z);
        min_z = Mathf.Min(rangePoint1.transform.position.z, rangePoint2.transform.position.z);

        max_y = Mathf.Max(rangePoint1.transform.position.y, rangePoint2.transform.position.y);
        min_y = Mathf.Min(rangePoint1.transform.position.y, rangePoint2.transform.position.y);
    }
    public void Reset()
    {
        float x = Random.Range(min_x, max_x);
        float y = Random.Range(min_y, max_y);
        float z = Random.Range(min_z, max_z);

        transform.position = new Vector3(x, y, z);  
    }
}
