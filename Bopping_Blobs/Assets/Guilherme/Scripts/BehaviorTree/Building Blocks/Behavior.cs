namespace BehaviorTree {
    public enum EReturnStatus {
        SUCCESS,
        FAILURE
    }

    /// <summary>
    /// Behavior is an abstract interface that can activated, run, and deactivated.
    /// </summary>
    public abstract class Behavior {
        private string m_nodeName;

        public Behavior(string _nodeName) {
            m_nodeName = _nodeName;
        }

        /// <summary>
        /// Update the Behavior. Should be called once every time the behavior tree is updated, until it signals it has terminated.
        /// </summary>
        /// <returns>Behavior Status (SUCCESS, FAILURE, RUNNING or SUSPENDED)</returns>
        public abstract EReturnStatus Update();
    }
}
