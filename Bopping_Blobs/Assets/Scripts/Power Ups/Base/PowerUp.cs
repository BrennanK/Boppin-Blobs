using UnityEngine;

namespace PowerUp {
    [System.Serializable]
    public abstract class PowerUp {
        public string powerUpName;
        public bool hasDuration;
        public float duration;
        public float radius;

        public PowerUp(string _powerUpName, bool _hasDuration, float _duration, float _radius) {
            powerUpName = _powerUpName;
            hasDuration = _hasDuration;
            duration = _duration;
            radius = _duration;
        }

        public abstract void ActivatePowerUp();
        public abstract void ResetEffects();
    }
}
