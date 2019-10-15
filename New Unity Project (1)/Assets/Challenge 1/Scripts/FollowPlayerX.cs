using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerX : MonoBehaviour
{
    public GameObject plane;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(-50, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = plane.transform.position + offset;
        transform.Rotate(Vector3.up, 0.05f);
    }
}
