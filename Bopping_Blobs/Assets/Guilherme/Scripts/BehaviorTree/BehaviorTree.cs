using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree {
    // TODO load the tree from a file?
    public class BehaviorTree {
        protected Behavior m_treeRoot;

        public BehaviorTree(Behavior _treeRoot) {
            m_treeRoot = _treeRoot;
        }

        public void Update() {
            Debug.Log($"{m_treeRoot.Update()}");
        }
    }
}
