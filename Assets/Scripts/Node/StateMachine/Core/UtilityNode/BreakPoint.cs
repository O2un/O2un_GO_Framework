using UnityEngine;

namespace O2un.Node.StateMachine
{
    public class BreakPoint : UtilityNode
    {
        protected override void OnStart()
        {
            Debug.Break();
        }

        protected override void OnStop()
        {
        }

        protected override NodeState OnUpdate()
        {
            return NodeState.Success;
        }
    }
}
