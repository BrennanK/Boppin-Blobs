using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class ThirdPersonInput : MonoBehaviour
{
    private DigitalJoystick DJ;
    private JoyButton JB;

    private ThirdPersonUserControl TPUC;

    private ThirdPersonCharacter TPC;

    public float groundCheckDistance = 0.2f;

    public float sphereCastRadius = 0.31f;

    public float qubedistance = 0.2f;
    
    void Start()
    {
        DJ = FindObjectOfType<DigitalJoystick>();
        JB = FindObjectOfType<JoyButton>();
        TPUC = FindObjectOfType<ThirdPersonUserControl>();
        TPC = FindObjectOfType<ThirdPersonCharacter>();
    }

    void Update()
    {
        TPUC.m_Jump = JB.pressed;
        TPUC.Hinput = DJ.Horizontal;
        TPUC.Vinput = DJ.Vertical;
        if (TPC.addGravity)
        {
            CheckRayCastHit();
        }
    }
    void CheckRayCastHit()
    {
        RaycastHit hit;



        if (Physics.SphereCast(transform.position + (Vector3.up * qubedistance), sphereCastRadius, Vector3.down, out hit, groundCheckDistance))
        {
            if (hit.transform.GetComponent<PlayerTag>())
            {
                Debug.Log("Hit Other Player Name:" + hit.transform.gameObject.name);
                hit.transform.GetComponent<PlayerTag>().GetRayCastHit();
            }
        }

        //if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hit, groundCheckDistance))
        //{
        //    if (hit.transform.GetComponent<PlayerTag>())
        //    {
        //        Debug.Log("Hit Other Player");
        //        hit.transform.GetComponent<PlayerTag>().GetRayCastHit();
        //    }
        //}

    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position+(Vector3.up * qubedistance), sphereCastRadius);
        //transform.position + (Vector3.up * qubedistance) + (Vector3.down * sphereCastRadius)
        //Gizmos.DrawCube(transform.position+(Vector3.up * qubedistance), new Vector3(size,size,size));
        Gizmos.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * 0.2f));
        //Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * 10));
    }
}
