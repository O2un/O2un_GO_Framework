#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace O2un.Core
{
    #if ODIN_INSPECTOR
    public class O2unScriptableObject : SerializedScriptableObject
    {
    }
    #else
    public class O2unScriptableObject : ScriptableObject
    {
    }
    #endif
}
