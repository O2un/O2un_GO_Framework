using UnityEngine;
using XNodeEditor;

namespace O2un.Node.StateMachine
{
    [CustomNodeGraphEditor(typeof(StateMachineGraph))]
    public class StateMachineEditor : NodeGraphEditor
    {
        StateMachineGraph _stm;
        public override void OnGUI()
        {
            base.OnGUI();

            _stm = target as StateMachineGraph;
        }

        public override void OnWindowFocus()
        {
            base.OnWindowFocus();

            if(null != _stm)
            {
                if(null == _stm.Root)
                {
                    _stm.SetRoot(CreateNode(typeof(RootNode), new()));
                }
            }
        }
    }
}
