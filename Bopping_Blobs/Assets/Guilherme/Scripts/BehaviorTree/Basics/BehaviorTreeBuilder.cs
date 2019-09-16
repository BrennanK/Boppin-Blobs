using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree {
    public delegate EReturnStatus BehaviorTreeAction();

    public class BehaviorTreeBuilder {
        private Behavior m_currentNode = null;
        private Stack<Composite> parentNodeStack = new Stack<Composite>();

        public BehaviorTreeBuilder Sequence() {
            Sequence newSequenceNode = new Sequence();

            if(parentNodeStack.Count > 0) {
                parentNodeStack.Peek().AddChildBehavior(newSequenceNode);
            }

            parentNodeStack.Push(newSequenceNode);
            return this;
        }

        public BehaviorTreeBuilder Selector() {
            Selector newSelectorNode = new Selector();

            if(parentNodeStack.Count > 0) {
                parentNodeStack.Peek().AddChildBehavior(newSelectorNode);
            }

            parentNodeStack.Push(newSelectorNode);
            return this;
        }

        public BehaviorTreeBuilder Action(string _name, BehaviorTreeAction _functionToExecute) {
            if(parentNodeStack.Count < 0) {
                Debug.LogError($"[BEHAVIOR TREE BUILDER] Trying to insert Action Node when there is no available parent");
                return this;
            }

            Action actionNode = new Action(_name, _functionToExecute);
            parentNodeStack.Peek().AddChildBehavior(actionNode);
            return this;
        }

        public BehaviorTreeBuilder Condition(string _name, BehaviorTreeAction _functionToExecute) {
            if(parentNodeStack.Count < 0) {
                Debug.LogError($"[BEHAVIOR TREE BUILDER] Trying to insert Action Node where there is no available parent");
                return this;
            }

            Condition conditionNode = new Condition(_name, _functionToExecute);
            parentNodeStack.Peek().AddChildBehavior(conditionNode);
            return this;
        }

        public BehaviorTreeBuilder End() {
            m_currentNode = parentNodeStack.Pop();
            return this;
        }

        public Behavior Build() {
            if(m_currentNode == null) {
                Debug.LogError($"[BEHAVIOR TREE BUILDER] Cannot create a tree with zero nodes!");
            }

            return m_currentNode;
        }
    }
}
