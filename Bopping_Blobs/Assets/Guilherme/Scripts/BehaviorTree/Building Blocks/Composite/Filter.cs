namespace BehaviorTree {
    /// <summary>
    /// A filter is a branch that will execute its child behavior under specific conditions
    /// </summary>
    public abstract class Filter : Sequence {
        public Filter(string _nodeName) : base(_nodeName) {
            // TODO
        }

        public void AddCondition(Behavior _condition) {
            m_childrenBehaviors.Insert(0, _condition);
        }

        public void AddAction(Behavior _action) {
            base.AddChildBehavior(_action);
        }
    }
}
