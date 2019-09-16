using System.Collections.Generic;

namespace BehaviorTree {
    /// <summary>
    /// Composites are nodes that can have multiple children.
    /// </summary>
    public abstract class Composite : Behavior {
        protected List<Behavior> m_childrenBehaviors = new List<Behavior>();

        public Composite(string _nodeName) : base(_nodeName) {
            // TODO
        }

        public void AddChildBehavior(Behavior _behavior) {
            m_childrenBehaviors.Add(_behavior);
        }
    }
}
