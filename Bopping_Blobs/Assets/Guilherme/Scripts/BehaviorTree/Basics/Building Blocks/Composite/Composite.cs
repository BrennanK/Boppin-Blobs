using System.Collections.Generic;

namespace BehaviorTree {
    /// <summary>
    /// Composites are nodes that can have multiple children.
    /// </summary>
    public abstract class Composite : Behavior {
        protected List<Behavior> m_childrenBehaviors = new List<Behavior>();

        public override void Initialize() {
            foreach (Behavior behavior in m_childrenBehaviors) {
                behavior.Initialize();
            }
        }

        public override void Terminate(EReturnStatus _status) {
            foreach (Behavior behavior in m_childrenBehaviors) {
                behavior.Terminate(_status);
            }
        }

        public void AddChildBehavior(Behavior _behavior) {
            m_childrenBehaviors.Add(_behavior);
        }

        public void RemoveChildBehavior(Behavior _behavior) {
            m_childrenBehaviors.Remove(_behavior);
        }

        public void ClearAllChildren() {
            m_childrenBehaviors.Clear();
        }
    }
}
