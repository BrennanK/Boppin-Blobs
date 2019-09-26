using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerUp {
    public class PowerUpHolder {
        public PowerUp powerUp;
        public bool activated;
        public bool canActivate;
        public float powerUpTimer;
        // UI Element

        public PowerUpHolder() {
            powerUp = null;
            activated = false;
            canActivate = true;
            powerUpTimer = 0f;
        }
    }

    public class PowerUpTracker : MonoBehaviour {
        public PowerUpHolder slot1 = new PowerUpHolder();
        public PowerUpHolder slot2 = new PowerUpHolder();
        private bool m_isPlayer;
        private UIManager m_UIManager;

        private void Start() {
            m_UIManager = FindObjectOfType<UIManager>();

            if(this.tag == "Player") {
                m_isPlayer = true;
            }

            if(m_isPlayer) {
                m_UIManager.UpdatePowerUpUI(slot1, slot2);
            }
        }

        private void Update() {
            CheckPowerUp(slot1);
            CheckPowerUp(slot2);
        }

        private void CheckPowerUp(PowerUpHolder _slot) {
            if(_slot.activated) {
                _slot.powerUpTimer -= Time.deltaTime;

                if (_slot.powerUpTimer <= 0) {
                    _slot.activated = false;
                    _slot.powerUp.ResetEffects();
                    _slot.powerUp = null;
                }

                if(m_isPlayer) {
                    m_UIManager.UpdatePowerUpUI(slot1, slot2);
                }
            }
        }

        public void AddPowerUp(PowerUp _powerUp) {
            if (slot1.powerUp == null) {
                Debug.Log($"Adding power up to slot 1");
                slot1.powerUp = _powerUp;
                slot1.canActivate = true;
            } else if(slot2.powerUp == null) {
                Debug.Log($"Adding power up to slot 2");
                slot2.powerUp = _powerUp;
                slot2.canActivate = true;
            }

            if(m_isPlayer) {
                m_UIManager.UpdatePowerUpUI(slot1, slot2);
            }
        }

        public void ActivatePowerUp1() {
            ActivatePowerUp(slot1);
        }

        public void ActivatePowerUp2() {
            ActivatePowerUp(slot2);
        }

        private void ActivatePowerUp(PowerUpHolder _slot) {
            if(_slot.canActivate) {
                if(_slot.powerUp != null) {

                    if(_slot.powerUp.hasDuration) {
                        _slot.powerUpTimer = _slot.powerUp.duration;
                        _slot.activated = true;
                    } else {
                        _slot.powerUp = null;
                    }

                    _slot.canActivate = false;
                    _slot.powerUp.ActivatePowerUp();

                    if(m_isPlayer) {
                        m_UIManager.UpdatePowerUpUI(slot1, slot2);
                    }
                }
            }
        }

    }
}
