using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace O2un.Node.StateMachine
{
	public enum NodeState
	{
		Success,
		Failure,
		Running,
	}

	[Serializable]
	public class EmptyNode { }
		
	public abstract class StateNode : XNode.Node 
	{
		[Input] public EmptyNode _enter;
		[Output] public EmptyNode _exit;

		protected override void Init() {
			base.Init();

			State = NodeState.Running;
		}

		public NodeState State { get; private set; } = NodeState.Running;
        bool _isStarted;
        public bool Started => _isStarted;
        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract NodeState OnUpdate();

		public NodeState Update()
		{
			if (false == _isStarted)
            {
                OnStart();
                _isStarted = true;
            }

            State = OnUpdate();

            if (NodeState.Running != State)
            {
                OnStop();
                _isStarted = false;
            }

            return State;
		}

		public StateNode GetNextNode() {
			StateMachineGraph fmGraph = graph as StateMachineGraph;

			if (fmGraph._current != this) {
				Debug.LogWarning("Node isn't active");
				return null;
			}

			NodePort exitPort = GetOutputPort("exit");

			if (!exitPort.IsConnected) {
				Debug.LogWarning("Node isn't connected");
				return null;
			}

			return exitPort.Connection.node as StateNode;
		}
	}
}