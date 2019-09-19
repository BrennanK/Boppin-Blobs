namespace BehaviorTree {
    /// <summary>
    /// <para>Conditions are leaf nodes and it is the primary way the tree can check information in the world.</para>
    /// <para>They rely on the tree's return statuses (SUCCESS and FAILURE) to signal true and false.</para>
    /// </summary>
    public class Condition : Behavior {
        private BehaviorTreeAction m_nodeBehavior;

        public Condition(string _nodeName, BehaviorTreeAction _nodeBehavior) : base(_nodeName) {
            m_nodeBehavior = _nodeBehavior;
        }

        public override EReturnStatus Update() {
            return m_nodeBehavior();
        }
    }
}
