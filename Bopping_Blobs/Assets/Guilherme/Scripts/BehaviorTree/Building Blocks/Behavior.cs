using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree {
    public enum EReturnStatus {
        SUCCESS,
        FAILURE,
        RUNNING,
        SUSPENDED
    }

    public abstract class Behavior {
        // Initialize() method should be called once and before any call for the Update() method.
        public abstract void Initialize();
        // Update() method should be called once every time the behavior tree is updated, until it signals it has terminated
        public abstract EReturnStatus Update();
        // Terminate() is called once after the Update() return a signal that it is no longer running
        public abstract void Terminate(EReturnStatus _status);
    }
}
