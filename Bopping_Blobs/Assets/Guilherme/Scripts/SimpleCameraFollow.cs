using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour  {
    public Transform toFollow;
    private float zOffset;

    private void Awake() {
        zOffset = transform.position.z - toFollow.position.z;
    }

    private void Update() {
        transform.position = new Vector3(toFollow.position.x, transform.position.y, toFollow.position.z + zOffset);
    }
}
