namespace BehaviorTree {
    // Sequences allows Behavior Trees to follow "plans" that are specified by the designers
    public abstract class Sequence : Composite {
        // Sequence Update runs all its children Update functions until one of them fails or is still running
        // Returns Success when all children returned success, otherwise, returns the child status that was different than success.
        public override EReturnStatus Update() {
            foreach(Behavior childBehavior in m_childrenBehaviors) {
                EReturnStatus childStatus = childBehavior.Update();

                if (childStatus != EReturnStatus.SUCCESS) {
                    return childStatus;
                }
            }

            return EReturnStatus.SUCCESS;
        }
    }
}
