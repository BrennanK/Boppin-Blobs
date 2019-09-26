using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerUp {
    public class TestPowerUp : PowerUp {
        public TestPowerUp(string _powerUpName, bool _hasDuration, float _duration, float _radius) : base(_powerUpName, _hasDuration, _duration, _radius) { }

        public override void ActivatePowerUp() {
            Debug.Log($"{this} is activated!");
        }

        public override void ResetEffects() {
            Debug.Log($"{this} is reset!");
        }
    }
}
