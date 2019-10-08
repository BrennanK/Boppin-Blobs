using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundBlobs : MonoBehaviour
{
    public Material[] blobMats;
    public Transform endPoint;
    Vector3 myPos;
    GameObject myBlob;
    // Start is called before the first frame update
    void Start()
    {
        myPos = transform.position;
        myBlob = GetComponentInChildren<Animator>().gameObject;
        myBlob.GetComponent<Renderer>().material = blobMats[Random.Range(0, blobMats.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, endPoint.position, .005f);
        if(Vector3.Distance(transform.position, endPoint.position) < 2f)
        {
            transform.position = myPos;
            myBlob.GetComponent<Renderer>().material = blobMats[Random.Range(0, blobMats.Length)];
        }
    }
}
