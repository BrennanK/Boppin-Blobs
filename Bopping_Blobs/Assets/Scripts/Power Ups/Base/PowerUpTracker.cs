using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PowerUp {
    public class PowerUpHolder {
        public PowerUp powerUp;
        public bool activated;
        public bool canActivate;
        public float powerUpTimer;

        public PowerUpHolder() {
            powerUp = null;
            activated = false;
            canActivate = true;
            powerUpTimer = 0f;
        }
    }

    public enum EPowerUps {
        SUPER_SLAM = 1,
        SUPER_SPEED = 2,
        BACK_OFF = 3,
    }

    public class PowerUpTracker : MonoBehaviour {
        public PowerUpHolder slot1 = new PowerUpHolder();
        public PowerUpHolder slot2 = new PowerUpHolder();

        private TaggingManager m_taggingManager;
        private TaggingIdentifier m_playerTaggingIdentifier;
        private IBoppable m_boppableInterface;
        private bool m_isPlayer;
        private UIManager m_UIManager;

        private void Start() {
            m_playerTaggingIdentifier = GetComponent<TaggingIdentifier>();
            m_taggingManager = FindObjectOfType<TaggingManager>();
            m_boppableInterface = GetComponent<IBoppable>();
            m_UIManager = FindObjectOfType<UIManager>();

            if(this.tag == "Player") {
                m_isPlayer = true;
            }

            if(m_isPlayer) {
                m_UIManager.UpdatePowerUpUI(slot1, slot2);
            }
        }

        private void Update() {
            if(Input.GetKeyDown(KeyCode.Q)) {
                ActivatePowerUp1();
            }

            if(Input.GetKeyDown(KeyCode.E)) {
                ActivatePowerUp2();
            }

            CheckPowerUp(ref slot1);
            CheckPowerUp(ref slot2);
        }

        private void CheckPowerUp(ref PowerUpHolder _slot) {
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
            Debug.Log($"Power Up Received by Power Up Tracker: {_powerUp.GetHashCode()}");
            switch(_powerUp.powerUp) {
                case EPowerUps.SUPER_SPEED:
                    _powerUp.activatePowerUpAction += ActivateSuperSpeed;
                    _powerUp.resetPowerUpAction += ResetSuperSpeed;
                    break;
                case EPowerUps.SUPER_SLAM:
                    _powerUp.activatePowerUpAction += ActivateSuperSlam;
                    break;
                case EPowerUps.BACK_OFF:
                    _powerUp.activatePowerUpAction += ActivateBackOff;
                    break;
            }

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
            ActivatePowerUp(ref slot1);
        }

        public void ActivatePowerUp2() {
            ActivatePowerUp(ref slot2);
        }

        private void ActivatePowerUp(ref PowerUpHolder _slot) {
            if(_slot.canActivate) {
                if(_slot.powerUp != null) {
                    _slot.canActivate = false;
                    _slot.powerUp.ActivatePowerUp();

                    if (_slot.powerUp.hasDuration) {
                        _slot.powerUpTimer = _slot.powerUp.duration;
                        _slot.activated = true;
                    } else {
                        _slot.powerUp.ResetEffects();
                        _slot.powerUp = null;
                    }

                    if(m_isPlayer) {
                        m_UIManager.UpdatePowerUpUI(slot1, slot2);
                    }
                }
            }
        }

        #region Power Up Functions
        // TODO
        // ATTENTION!! IMPORTANT!!
        // IF SOMEONE HAVE ACTIVATE SUPER SPEED AND GET TAGGED, THEIR SPEED WILL BE SET TO THE NORMAL AFTER THAT, AND THEN THE SUPER SPEED WILL RESET, MAKING THEIR BASE SPEED LOWER THAN WHAT IT SHOULD BE!!!
        public void ActivateSuperSpeed(float _value) {
            Debug.Log($"Activating Super Speed ({m_boppableInterface.GetSpeed()}, {m_taggingManager.baseSpeed}, {_value}) - Speed will be {m_boppableInterface.GetSpeed() + (m_taggingManager.baseSpeed * _value)}");
            m_boppableInterface.ChangeSpeed(m_boppableInterface.GetSpeed() + (m_taggingManager.baseSpeed * _value));
        }

        public void ResetSuperSpeed(float _value) {
            Debug.Log($"Resetting Super Speed ({m_boppableInterface.GetSpeed()}, {m_taggingManager.baseSpeed}, {_value}) - Speed will be {m_boppableInterface.GetSpeed() - (m_taggingManager.baseSpeed * _value)}");
            m_boppableInterface.ChangeSpeed(m_boppableInterface.GetSpeed() - (m_taggingManager.baseSpeed * _value));
        }

        public void ActivateBackOff(float _value) {
            List<TaggingIdentifier> taggingIdentifiers = FindObjectsOfType<TaggingIdentifier>().ToList();
            taggingIdentifiers.Remove(this.GetComponent<TaggingIdentifier>());

            foreach(TaggingIdentifier player in taggingIdentifiers) {
                if(Vector3.Distance(player.transform.position, transform.position) < m_taggingManager.knockbackRadius) {
                    player.KnockbackPlayer(Color.magenta, (player.transform.position - transform.position).normalized * m_taggingManager.knockbackForce * 2f, _value);
                }
            }
        }

        public void ActivateSuperSlam(float _value) {
            m_playerTaggingIdentifier.ForceAttackWithMultiplier(_value);
        }
        #endregion

    }
}
