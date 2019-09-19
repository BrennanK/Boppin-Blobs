using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree {
    public class BehaviorTree {
        protected Behavior m_treeRoot;

        public BehaviorTree(Behavior _treeRoot) {
            m_treeRoot = _treeRoot;
        }

        public void Update() {
            if(m_treeRoot == null) {
                Debug.LogError("Behavior Tree Root is null! Are you sure you built it correctly?");
                return;
            }

            m_treeRoot.Update();
        }
    }
}
