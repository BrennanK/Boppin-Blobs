namespace BehaviorTree {
    /// <summary>
    /// Decorator wraps a behavior with another behavior.
    /// </summary>
    public abstract class Decorator : Behavior {
        protected Behavior m_child;

        public Decorator(Behavior _child) {
            m_child = _child;
        }
    }
}
