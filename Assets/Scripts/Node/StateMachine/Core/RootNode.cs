using System.Linq;
using UnityEngine;

namespace O2un.Node.StateMachine
{
    public class RootNode : StateNode
    {
        [Output(connectionType = ConnectionType.Override)] public Port _exit;

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override NodeState OnUpdate()
        {
            var exitPort = GetExitNode();
            return exitPort.Update();
        }
    }
}
