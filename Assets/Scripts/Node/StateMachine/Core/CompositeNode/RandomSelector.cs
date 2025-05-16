using O2un.Core;
using UnityEngine;

namespace O2un.Node.StateMachine
{
    public class Selector : CompositeNode
    {
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override NodeState OnUpdate()
        {
            var list = GetEixtList();
            var pick = list.PickOne();
            if(null == pick)
            {
                return NodeState.Failure;
            }

            return pick.Update();
        }
    }
}
