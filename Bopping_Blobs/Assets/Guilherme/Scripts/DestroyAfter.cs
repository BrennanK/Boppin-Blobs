using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour {
    public float timeToDestroy = 1.0f;

    private void Start() {
        StartCoroutine(WaitToDestroy(timeToDestroy));
    }

    private IEnumerator WaitToDestroy(float _delay) {
        yield return new WaitForSeconds(_delay);
        Destroy(gameObject);
    }
}
