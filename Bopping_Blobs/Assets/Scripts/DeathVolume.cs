using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathVolume : MonoBehaviour
{
    PowerUp.PowerUpTracker[] players;
    Vector3[] startPoints;
    // Start is called before the first frame update
    void Start()
    {
        
        players = new PowerUp.PowerUpTracker[FindObjectsOfType<PowerUp.PowerUpTracker>().Length];
        players = FindObjectsOfType<PowerUp.PowerUpTracker>();

        startPoints = new Vector3[players.Length];
        for(int i = 0; i < startPoints.Length; i++)
        {
            startPoints[i] = players[i].transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        for(int i = 0; i < startPoints.Length; i++)
        {
            if(other.GetComponentInParent<PowerUp.PowerUpTracker>().gameObject == players[i].gameObject)
            {
                other.GetComponent<Rigidbody>().velocity = Vector3.zero;
                other.GetComponent<CharacterController>().enabled = false;
                other.GetComponentInParent<PowerUp.PowerUpTracker>().transform.position = startPoints[i];
                other.GetComponent<CharacterController>().enabled = true;
            }
        }
    }
}
