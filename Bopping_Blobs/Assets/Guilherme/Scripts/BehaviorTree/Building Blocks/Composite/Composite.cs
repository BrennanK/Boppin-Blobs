using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree {
    public abstract class Composite : Behavior {
        protected List<Behavior> m_childrenBehaviors;

        public override void Initialize() {
            m_childrenBehaviors = new List<Behavior>();
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
