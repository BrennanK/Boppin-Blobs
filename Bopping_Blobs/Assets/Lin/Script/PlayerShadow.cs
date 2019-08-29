using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerShadow : MonoBehaviour
{

    public Transform player;

    private Vector3 offset;

    private ThirdPersonCharacter TPC;

    public GameObject jumpDistance;

    public GameObject shadow;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.position;
        TPC = player.gameObject.GetComponent<ThirdPersonCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerRot = player.rotation.eulerAngles;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x,playerRot.y,transform.eulerAngles.z);
        Vector3 canvasPosition = player.position + offset;
        transform.position = new Vector3(canvasPosition.x, transform.position.y, canvasPosition.z);
        if (TPC.m_IsGrounded)
        {
            jumpDistance.SetActive(true);
            shadow.SetActive(false);
        }
        else
        {
            jumpDistance.SetActive(false);
            shadow.SetActive(true);
        }

    }
}
