using O2un.Core.Utils;
using UnityEngine;

namespace O2un.Node.StateMachine
{
    public class Log : UtilityNode
    {
        public string _log;
        protected override void OnStart()
        {
            LogHelper.Log( LogHelper.LogLevel.Debug, _log);
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
