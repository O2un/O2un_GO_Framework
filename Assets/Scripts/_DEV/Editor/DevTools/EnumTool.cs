using System.Text;
using O2un.Config;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

[O2un.Config.GlobalConfig]
public class EnumTool : ConfigHelper<EnumTool>
{
    private void ShowInspector()
    {
        tags = UnityEditorInternal.InternalEditorUtility.tags;
        layers = UnityEditorInternal.InternalEditorUtility.layers;
    }

#if ODIN_INSPECTOR
    [BoxGroup("유니티 태그 레이어")]
    [ReadOnly,ShowInInspector,OnInspectorGUI("ShowInspector")]
#endif
    string[] tags;

#if ODIN_INSPECTOR
    [BoxGroup("유니티 태그 레이어")]
    [ReadOnly,ShowInInspector,OnInspectorGUI("ShowInspector")]
#endif
    string[] layers;

#if ODIN_INSPECTOR
    [BoxGroup("유니티 태그 레이어")]
    [Button(name: "Tag & Layer Regeneration")]
#endif
    private void TagLayerGenerator()
    {
        StringBuilder stb = new();

        stb.AppendLine("public enum UnityTags");
        stb.AppendLine("{");

        var tags = UnityEditorInternal.InternalEditorUtility.tags;
        foreach(var tag in tags)
        {
            stb.Append(tag.Replace(" ","__"));
            stb.AppendLine(",");
        }

        stb.AppendLine("}");

        stb.AppendLine("public enum UnityLayers");
        stb.AppendLine("{");

        var layers = UnityEditorInternal.InternalEditorUtility.layers;
        foreach(var tag in layers)
        {
            stb.Append(tag.Replace(" ","__"));
            stb.AppendLine(",");
        }

        stb.AppendLine("}");


        string path = Application.dataPath + "/Scripts/Core/Defines/TagLayerList.cs";
        using(System.IO.StreamWriter sw = new(path,false))
        {
            sw.Write(stb.ToString());
        }
    }
}
