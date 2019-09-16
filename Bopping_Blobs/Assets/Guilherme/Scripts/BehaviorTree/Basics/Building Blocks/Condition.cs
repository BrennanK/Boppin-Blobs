namespace BehaviorTree {
    /// <summary>
    /// Conditions are leaf nodes and it is the primary way the tree can check information in the world.
    /// They rely on the tree's return statuses (SUCCESS and FAILURE) to signal true and false
    /// </summary>
    public class Condition : Behavior {
        private string m_nodeName;
        private BehaviorTreeAction m_nodeBehavior;

        public Condition(string _nodeName, BehaviorTreeAction _nodeBehavior) {
            m_nodeName = _nodeName;
            m_nodeBehavior = _nodeBehavior;
        }

        public override EReturnStatus Update() {
            return m_nodeBehavior();
        }

        public override void Initialize() {
            throw new System.NotImplementedException();
        }

        public override void Terminate(EReturnStatus _status) {
            throw new System.NotImplementedException();
        }
    }
}
