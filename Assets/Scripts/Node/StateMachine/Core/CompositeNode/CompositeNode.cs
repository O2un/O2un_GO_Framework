using UnityEngine;

namespace O2un.Node.StateMachine
{
    public abstract class CompositeNode : StateNode
    {
        [Input] public Port _enter;
        [Output(connectionType = ConnectionType.Override, dynamicPortList = true)] public Port _exit;
    }
}
