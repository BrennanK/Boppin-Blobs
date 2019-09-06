using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree {
    // A filter is a branch that will not execute its child behavior under specific conditions
    public abstract class Filter : Sequence {
        public void AddCondition(Behavior _condition) {
            m_childrenBehaviors.Insert(0, _condition);
        }

        public void AddAction(Behavior _action) {
            base.AddChildBehavior(_action);
        }
    }
}
