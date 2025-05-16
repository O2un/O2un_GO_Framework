using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace O2un.Node.StateMachine
{
    public abstract class StateMachineGraph : NodeGraph 
    {
        public NodeState State {get; private set;} = NodeState.Running;
        [SerializeField] private RootNode _root;
        public RootNode Root => _root;
        
        public void SetRoot(XNode.Node node)
        {
            _root = node as RootNode;
        }

		public NodeState Update() 
        {
            if(NodeState.Running == _root.State)
            {
                State = _root.Update();
            }

            return State;
        }
    }
}