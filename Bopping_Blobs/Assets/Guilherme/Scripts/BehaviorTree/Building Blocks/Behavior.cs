namespace BehaviorTree {
    public enum EReturnStatus {
        SUCCESS,
        FAILURE,
        RUNNING,
        SUSPENDED
    }

    /// <summary>
    /// Behavior is an abstract interface that can activated, run, and deactivated.
    /// </summary>
    public abstract class Behavior {
        /// <summary>
        /// Initialize the Behavior. It is called once and before any call for the Update() method.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Update the Behavior. Should be called once every time the behavior tree is updated, until it signals it has terminated.
        /// </summary>
        /// <returns>Behavior Status (SUCCESS, FAILURE, RUNNING or SUSPENDED)</returns>
        public abstract EReturnStatus Update();

        /// <summary>
        /// Terminate the Behavior. Should be called once after the Update() returns a signal that it is no longer running.
        /// </summary>
        /// <param name="_status"></param>
        public abstract void Terminate(EReturnStatus _status);
    }
}
