using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree {
    public delegate EReturnStatus BehaviorTreeAction();

    public class BehaviorTreeBuilder {
        /// <summary>
        /// Last node that was created in the tree.
        /// </summary>
        private Behavior m_currentNode = null;

        /// <summary>
        /// Parent nodes stack, so we know where the nodes are being inserted
        /// </summary>
        private Stack<Composite> parentNodeStack = new Stack<Composite>();

        /// <summary>
        /// <para>Inserts a Sequence into the tree.</para>
        /// <para>A sequence executes its children until one of them fails.</para>
        /// </summary>
        /// <param name="_nodeName">Name to describe the Sequence</param>
        /// <returns>Behavior Tree builder state</returns>
        public BehaviorTreeBuilder Sequence(string _nodeName) {
            Sequence newSequenceNode = new Sequence(_nodeName);

            if(parentNodeStack.Count > 0) {
                parentNodeStack.Peek().AddChildBehavior(newSequenceNode);
            }

            parentNodeStack.Push(newSequenceNode);
            return this;
        }

        /// <summary>
        /// <para>Insert a Selector into the tree.</para>
        /// <para>A selector executes its children until it finds a success.</para>
        /// </summary>
        /// <param name="_nodeName">Name to describe the selector</param>
        /// <returns>Behavior Tree builder state</returns>
        public BehaviorTreeBuilder Selector(string _nodeName) {
            Selector newSelectorNode = new Selector(_nodeName);

            if(parentNodeStack.Count > 0) {
                parentNodeStack.Peek().AddChildBehavior(newSelectorNode);
            }

            parentNodeStack.Push(newSelectorNode);
            return this;
        }

        // TODO Parallel

        /// <summary>
        /// Inserts an Action into the Behavior tree
        /// </summary>
        /// <param name="_name">Name describing the action</param>
        /// <param name="_functionToExecute">Function that the action will execute</param>
        /// <returns>Behavior Tree builder state</returns>
        public BehaviorTreeBuilder Action(string _name, BehaviorTreeAction _functionToExecute) {
            if(parentNodeStack.Count < 0) {
                Debug.LogError($"[BEHAVIOR TREE BUILDER] Trying to insert Action Node when there is no available parent");
                return this;
            }

            Action actionNode = new Action(_name, _functionToExecute);
            parentNodeStack.Peek().AddChildBehavior(actionNode);
            return this;
        }

        /// <summary>
        /// Inserts a Condition into the Behavior Tree
        /// </summary>
        /// <param name="_name">Name describing the condition</param>
        /// <param name="_functionToExecute">Function that the condition will execute</param>
        /// <returns>Behavior Tree builder state</returns>
        public BehaviorTreeBuilder Condition(string _name, BehaviorTreeAction _functionToExecute) {
            if(parentNodeStack.Count < 0) {
                Debug.LogError($"[BEHAVIOR TREE BUILDER] Trying to insert Action Node where there is no available parent");
                return this;
            }

            Condition conditionNode = new Condition(_name, _functionToExecute);
            parentNodeStack.Peek().AddChildBehavior(conditionNode);
            return this;
        }

        /// <summary>
        /// Ends a sequence or selector.
        /// </summary>
        /// <returns>Behavior Tree builder state</returns>
        public BehaviorTreeBuilder End() {
            m_currentNode = parentNodeStack.Pop();
            return this;
        }

        /// <summary>
        /// Builds the actual tree.
        /// </summary>
        /// <returns>Root noode</returns>
        public Behavior Build() {
            if(m_currentNode == null) {
                Debug.LogError($"[BEHAVIOR TREE BUILDER] Cannot create a tree with zero nodes!");
            }

            return m_currentNode;
        }
    }
}
