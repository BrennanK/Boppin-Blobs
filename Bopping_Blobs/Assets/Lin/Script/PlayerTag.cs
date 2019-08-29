using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerTag : MonoBehaviour
{
    public GameObject tagCanvas;

    public TagManager TM;

    public int playerNumber;

    private Rigidbody rb;

    public float bountFource = 1f;

    public PhysicMaterial iceyMaterial;

    private Collider playerCollider;
    [Header("How long the effect")]
    public float iceyTime;

    private bool ishitted;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
    void OnCollisionEnter(Collision other)
    {
        //Debug.Log("Collider Enter");
        if (other.transform.GetComponent<ThirdPersonCharacter>())
        {
            if (other.transform.GetComponent<ThirdPersonCharacter>().addGravity == true)
            {
                TM.ChangeTag(playerNumber);
            }
        }
    }
    */
    public void GetRayCastHit()
    {
        if (!ishitted)
        {
            TM.ChangeTag(playerNumber);
            ishitted = true;
            //Invoke("ReTagOpen",0.5f);
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
        rb.AddForce(faceDirection, ForceMode.VelocityChange);
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
