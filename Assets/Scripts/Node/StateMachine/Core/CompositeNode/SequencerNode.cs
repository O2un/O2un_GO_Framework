using UnityEngine;

namespace O2un.Node.StateMachine
{
    public class SequencerNode : CompositeNode
    {
        int _currentNo = 0;
        protected override void OnStart()
        {
            _currentNo = 0;
        }

        protected override void OnStop()
        {
            
        }

        protected override NodeState OnUpdate()
        {
            var connList = GetEixtList();
            if(0 == connList.Count)
            {
                return NodeState.Failure;
            }

            var current = connList[_currentNo];
            switch (current.Update())
            {
                case NodeState.Success: ++_currentNo; break;
                case NodeState.Running: return NodeState.Running;
                case NodeState.Failure: return NodeState.Failure;
            }

            return _currentNo == connList.Count ? NodeState.Success : NodeState.Running;
        }
    }
}
