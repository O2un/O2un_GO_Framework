using UnityEngine;
using System.IO;
using O2un.Core.Utils;
using O2un.Network;
using System.Text;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

public class CustomMessageTool
{
    string cspath = Application.dataPath + "/Scripts/Network/Netcode/CustomMessage/CustomMessageManager.cs";
    string VALUEPREFIX = "//#STARTLIST";
    string VALUESUFFIX = "//#ENDLIST";

    #if ODIN_INSPECTOR
    [Button]
    #endif
    public void RegenerateScript()
    {
        var template = string.Empty;
        using (StreamReader sr = new(cspath))
        {
            template = sr.ReadToEnd();
        }
        
        var dispatchers = CommonUtils.GetTypes(t => t.IsClass && !t.IsAbstract && t.BaseType.IsGenericType 
            && (t.BaseType.GetGenericTypeDefinition() == typeof(ReqDispatcher<>) || t.BaseType.GetGenericTypeDefinition() == typeof(AckDispatcher<>)));
        
        StringBuilder values = new();
        values.AppendLine();

        foreach (var dispatcher in dispatchers)
        {
            var types = dispatcher.BaseType.GetGenericArguments();
            values.AppendLine($"            {{PayloadType.{types[0].Name}, new {dispatcher.Name}()}},");
        }

        var valueStart = template.IndexOfEnd(VALUEPREFIX);
        var valueEnd = template.IndexOf(VALUESUFFIX);

        template = template.Remove(valueStart, valueEnd - valueStart - 2);
        template = template.Insert(valueStart, values.ToString());

        using (StreamWriter sw = new(cspath, false))
        {
            sw.Write(template);
        }
    }
}
