using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerTag : MonoBehaviour
{
    public GameObject tagCanvas;
    public TagManager tagManager;
    public int playerNumber;
    private Rigidbody m_rigidbody;
    public float bountFource = 1f;
    public PhysicMaterial iceyMaterial;
    private Collider playerCollider;

    [Header("How long the effect")]
    public float iceyTime;
    private bool ishitted;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
    }

    public void GetRayCastHit()
    {
        if (!ishitted)
        {
            tagManager.ChangeTag(playerNumber);
            ishitted = true;
        }
    }

    void ReTagOpen()
    {
        ishitted = false;
    }

    public void ActivateTag()
    {
        tagCanvas.SetActive(true);
        playerCollider.material = iceyMaterial;
        float randomNum = Random.Range(0, 360);
        float trueAngle = randomNum / 180;
        Vector3 faceDirection = new Vector3(bountFource * Mathf.Sin(trueAngle * Mathf.PI), 0f, bountFource * Mathf.Cos(trueAngle * Mathf.PI));
        m_rigidbody.AddForce(faceDirection, ForceMode.VelocityChange);
        StartCoroutine(IceyEffect());
    }

    public void DisableTag()
    {
        tagCanvas.SetActive(false);
        ishitted = false;
    }

    IEnumerator IceyEffect()
    {
        yield return new WaitForSeconds(iceyTime);
        Debug.Log("Stop effect");
        playerCollider.material = null;
        ishitted = false;
        yield return null;
    }
}
