using UnityEngine;

namespace O2un.Node.StateMachine
{
    public abstract class ActionNode : StateNode
    {
        [Input(connectionType = ConnectionType.Multiple)] public Port _enter;
    }

    public abstract class UtilityNode : ActionNode
    {
        
    }
}
