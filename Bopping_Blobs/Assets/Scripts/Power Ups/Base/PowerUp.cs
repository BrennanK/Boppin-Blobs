using System;
using UnityEngine;

namespace PowerUp {
    public delegate void PowerUpAction(float _value);

    [System.Serializable]
    public class PowerUp {
        public string powerUpName;
        public bool hasDuration;
        public float duration;
        public float powerUpValue;
        public EPowerUps powerUp;
        public PowerUpAction activatePowerUpAction;
        public PowerUpAction resetPowerUpAction;

        public PowerUp(string _powerUpName, bool _hasDuration, float _duration, float _powerUpValue, EPowerUps _powerUp) {
            powerUpName = _powerUpName;
            hasDuration = _hasDuration;
            duration = _duration;
            powerUpValue = _powerUpValue;
            powerUp = _powerUp;
        }

        public void ActivatePowerUp() {
            activatePowerUpAction?.Invoke(powerUpValue);
        }

        public void ResetEffects() {
            resetPowerUpAction?.Invoke(powerUpValue);
        }
    }
}
