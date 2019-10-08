using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 rot;
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(rot);
    }
}
