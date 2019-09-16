namespace BehaviorTree {
    /// <summary>
    /// <para>An Action is a leaf node that have the responsibility of accessing information from the world and making changes to the world.</para>
    /// <para>When an action succeeds in making a change in the world, it returns EReturnStatus.SUCCESS, otherwise it returns EReturnStatus.FAILURE</para>
    /// </summary>
    public class Action : Behavior {
        private BehaviorTreeAction m_nodeBehavior;

        public Action(string _nodeName, BehaviorTreeAction _nodeBehavior) : base(_nodeName) {
            m_nodeBehavior = _nodeBehavior;
        }

        public override EReturnStatus Update() {
            return m_nodeBehavior();
        }
    }
}
