namespace BehaviorTree {
    /// <summary>
    /// Decorator wraps a behavior with another behavior.
    /// </summary>
    public abstract class Decorator : Behavior {
        protected Behavior m_child;

        public Decorator(string _nodeName, Behavior _child) : base(_nodeName) {
            m_child = _child;
        }
    }
}
