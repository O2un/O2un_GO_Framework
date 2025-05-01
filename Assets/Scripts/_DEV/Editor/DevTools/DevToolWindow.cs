using O2un.Data;
using UnityEditor;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
#endif

#if ODIN_INSPECTOR
public class DevToolWindow : OdinMenuEditorWindow
#else
public class DevToolWindow : EditorWindow
#endif
{
    [MenuItem("O2un/Dev Tools")]
    private static void OpenWindow()
    {
        var window = CreateInstance<DevToolWindow>();
        window.ShowUtility();
#if ODIN_INSPECTOR
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
#else
        window.position = new Rect(0, 0, 800, 500);
#endif
        window.titleContent = new GUIContent("Dev Tools");
    }
    
#if ODIN_INSPECTOR
    protected override OdinMenuTree BuildMenuTree()
    {
        this.MenuWidth = 270;
        this.WindowPadding = Vector4.zero;

        OdinMenuTree tree = new OdinMenuTree(false);
        tree.Config.DrawSearchToolbar = true;
        tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
        
        tree.AddAssetAtPath("Tools/Data List", DataConfig.Instance.GetPath());
        tree.Add("Tools/Data Tool", new DataTool());
        tree.AddAssetAtPath("Tools/Enum Tool", EnumTool.Instance.GetPath());
        //tree.Add("Tools/Navigation Tool", new NaviTool());
        return tree;
    }
#else
    private void OnGUI()
    {
        EditorGUILayout.HelpBox("Odin Inspector가 필요합니다.", MessageType.Warning);
    }
#endif
}
