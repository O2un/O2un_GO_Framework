using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace O2un.Node.StateMachine
{
    public abstract class StateMachineGraph : NodeGraph 
    {
        public StateNode _current;

		public void Update() 
        {
            if (null == _current)
            {
                return;
            }

            var state = _current.Update();

            switch (state)
            {
                case NodeState.Success: 
                    _current = _current.GetNextNode();
                    break;
                case NodeState.Failure: // 실패처리
                    break;
                case NodeState.Running: // 계속 진행
                    break;
            }
        }
    }
}