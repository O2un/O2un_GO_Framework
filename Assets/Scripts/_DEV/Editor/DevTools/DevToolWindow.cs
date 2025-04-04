using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class DevToolWindow : OdinMenuEditorWindow
{
    [MenuItem("O2un/Dev Tools")]
    private static void OpenWindow()
    {
        var window = CreateInstance<DevToolWindow>();
        window.ShowUtility();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        window.titleContent = new GUIContent("Dev Tools");
    }
    
    protected override OdinMenuTree BuildMenuTree()
    {
        this.MenuWidth = 270;
        this.WindowPadding = Vector4.zero;

        OdinMenuTree tree = new OdinMenuTree(false);
        tree.Config.DrawSearchToolbar = true;
        tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
        
        tree.AddAssetAtPath("Tools/Enum Tool", EnumTool.Instance.GetPath());
        //tree.Add("Tools/Navigation Tool", new NaviTool());
        return tree;
    }
}
