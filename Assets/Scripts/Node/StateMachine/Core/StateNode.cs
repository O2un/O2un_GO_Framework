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
	public class Port { }

    public class FailNode : StateNode
    {
        protected override void OnStart() {}
        protected override void OnStop() {}
        protected override NodeState OnUpdate()
        {
			return NodeState.Failure;
        }
    }

    public abstract class StateNode : XNode.Node 
	{
		// NOTE Input 과 Output Port의 이름은 _enter와 _exit로 고정
		// [Input] public Port _enter;
		// [Output] public Port _exit;

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

		private FailNode _failNode;
		private FailNode FAIL => _failNode ??= new();

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

		protected List<StateNode> GetEixtList()
		{
			List<StateNode> list = new();
			foreach(var port in DynamicOutputs)
			{
				if(port.Connection.node is StateNode node)
				{
					list.Add(node);
				}
			}
			return list;
		}

		protected StateNode GetExitNode()
		{
			var exit = GetOutputPort("_exit");
			if(false ==  exit.IsConnected)
			{
				return FAIL;
			}

			return exit.Connection.node as StateNode;
		}

        public override object GetValue(NodePort port)
        {
			return null;
        }
    }
}