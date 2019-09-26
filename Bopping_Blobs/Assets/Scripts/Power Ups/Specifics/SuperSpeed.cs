using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerUp {
    public class SuperSpeed : PowerUp {
        public SuperSpeed(string _powerUpName, bool _hasDuration, float _duration, float _radius) : base(_powerUpName, _hasDuration, _duration, _radius) { }

        public override void ActivatePowerUp() {
            throw new System.NotImplementedException();
        }

        public override void ResetEffects() {
            throw new System.NotImplementedException();
        }
    }
}
