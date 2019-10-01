using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerUp {
    [RequireComponent(typeof(MeshRenderer), typeof(Collider))]
    public class PowerUpBox : MonoBehaviour {
        private PowerUp[] m_powerUps;

        /*
        [SerializeField]
        private PowerUpEditor powerUpValues;
        */

        private bool m_isDisabled;
        private float m_disableTimerStart = 5f;
        private float m_disableTimer;
        private int m_itemNumber;

        private void Start() {
            m_powerUps = new PowerUp[3];
            m_powerUps[0] = new PowerUp("Super Speed", true, 5f, 0.25f, EPowerUps.SUPER_SPEED);
            m_powerUps[1] = new PowerUp("Back Off", false, 0.0f, 1f, EPowerUps.BACK_OFF);
            m_powerUps[2] = new PowerUp("Super Slam", false, 0.0f, 2f, EPowerUps.SUPER_SLAM);

            m_disableTimer = m_disableTimerStart;
        }

        private void Update() {
            if(m_isDisabled) {
                m_disableTimer -= Time.deltaTime;
            }
            
            if(m_disableTimer <= 0) {
                EnablePowerUp();
            }
        }

        private void OnTriggerEnter(Collider _other) {
            PowerUpTracker powerUpTracker = _other.GetComponent<PowerUpTracker>();

            if(powerUpTracker != null) {
                Debug.Log($"Power Up Box Collected!: {m_powerUps[0].GetHashCode()}");
                powerUpTracker.AddPowerUp(m_powerUps[Random.Range(0, m_powerUps.Length)].Clone());
                DisablePowerUp();
            }

        }

        private void EnablePowerUp() {
            m_isDisabled = false;
            m_disableTimer = m_disableTimerStart;

            // TODO cache reference
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            gameObject.GetComponent<Collider>().enabled = true;
        }

        private void DisablePowerUp() {
            m_isDisabled = true;

            // TODO cache reference
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }
}
