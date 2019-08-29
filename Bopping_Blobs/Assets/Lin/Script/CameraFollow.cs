using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    public float smoothing = 5f;

    private Vector3 offset;

    public bool lockX;

    public bool lockZ;
    [Header("Here set the player moving range when camera can follow")]
    public float borderX = 5.5f;

    public float borderZ = 15.5f;

    private Vector3 vectorX;

    private Vector3 vectorZ;

    private Vector3 fixedVector;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.position;
        Vector3 cameraPosition = player.position + offset;
        vectorX = new Vector3(cameraPosition.x, transform.position.y, transform.position.z);
        vectorZ = new Vector3(transform.position.x, transform.position.y, cameraPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(player.position.x) >= borderX)
        {
            lockX = true;
        }
        else
        {
            lockX = false;
        }

        if (Mathf.Abs(player.position.z)>= borderZ)
        {
            lockZ = true;
        }
        else
        {
            lockZ = false;
        }

        if (!lockX)
        {
            Vector3 cameraPosition = player.position + offset;
            //Vector3 movePositionX = new Vector3(cameraPosition.x, transform.position.y, transform.position.z);

            //transform.position = Vector3.Lerp(transform.position, movePositionX, smoothing * Time.deltaTime);
            vectorX = new Vector3(cameraPosition.x, transform.position.y, transform.position.z);

        }
        
        if (!lockZ)
        {
            Vector3 cameraPosition = player.position + offset;
            //Vector3 movePositionZ = new Vector3(transform.position.x, transform.position.y, cameraPosition.z);

            //transform.position = Vector3.Lerp(transform.position, movePositionZ, smoothing * Time.deltaTime);
            vectorZ = new Vector3(transform.position.x, transform.position.y, cameraPosition.z);
        }
        fixedVector = new Vector3(vectorX.x,transform.position.y,vectorZ.z);
        transform.position = Vector3.Lerp(transform.position, fixedVector, smoothing * Time.deltaTime);
    }
    
}
