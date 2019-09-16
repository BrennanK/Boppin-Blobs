namespace BehaviorTree {
    /// <summary>
    /// A Selector executes each of its child behaviors in order until it finds a child that either succeeds or that returns a RUNNING status.
    /// </summary>
    public class Selector : Composite {
        public Selector(string _nodeName) : base(_nodeName) {
            // TODO
        }

        public override EReturnStatus Update() {
            foreach(Behavior childBehavior in m_childrenBehaviors) {
                EReturnStatus childStatus = childBehavior.Update();

                if(childStatus != EReturnStatus.FAILURE) {
                    return childStatus;
                }
            }

            return EReturnStatus.FAILURE;
        }
    }
}
