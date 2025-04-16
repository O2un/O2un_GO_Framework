#if UNITY_EDITOR 
using System.Collections.Generic;
using System.IO;
using System.Linq;
using O2un.Core.Utils;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class PreviewScript : ScriptableObject
{
    public string fileName = "NewScript";
    [Title("미리보기", bold: false)]
    [HideLabel]
    [MultiLineProperty(30)]
    public string WideMultilineTextField = "";
}

public class NewScriptTool : OdinMenuEditorWindow
{
    public static readonly string CLASSNAME = "#SCRIPTNAME#";
    static List<TextAsset> scriptFileAssets = new();

    private string targetFolder;

    [MenuItem("Assets/O2un/Create Template Script", priority = -1000)]
    private static void ShowDialog()
    {
        var path = "Assets";
        var obj = Selection.activeObject;
        if (obj && AssetDatabase.Contains(obj))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!Directory.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }
        }

        var window = CreateInstance<NewScriptTool>();
        window.ShowUtility();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        window.titleContent = new GUIContent(path);
        window.targetFolder = path.Trim('/');

        if(0 == scriptFileAssets.Count)
        {
            scriptFileAssets = FileUtils.LoadAllAssetInFolder<TextAsset>(new[]{"Assets/Editor Default Resources/ScriptTemplates"});
        }
    }


    private PreviewScript previewObject;
    private Vector2 scroll;

    private TextAsset SelectedType
    {
        get
        {
            var m = this.MenuTree.Selection.LastOrDefault();
            return m == null ? null : m.Value as TextAsset;
        }
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        this.MenuWidth = 270;
        this.WindowPadding = Vector4.zero;

        OdinMenuTree tree = new OdinMenuTree(false);
        tree.Config.DrawSearchToolbar = true;
        tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
        tree.AddAllAssetsAtPath("Scripts","Assets/Editor Default Resources/ScriptTemplates",typeof(TextAsset),true);
        tree.SortMenuItemsByName();
        tree.Selection.SelectionConfirmed += x => this.CreateAsset();
        tree.Selection.SelectionChanged += e =>
        {
            if (this.previewObject && !AssetDatabase.Contains(this.previewObject))
            {
                DestroyImmediate(this.previewObject);
            }

            if (e != SelectionChangedType.ItemAdded)
            {
                return;
            }

            var t = this.SelectedType;
            if (t != null)
            {
                this.previewObject = CreateInstance(typeof(PreviewScript)) as PreviewScript;
                this.previewObject.WideMultilineTextField = t.text;
            }
        };
        
        return tree;
    }

    protected override IEnumerable<object> GetTargets()
    {
        yield return this.previewObject;
    }

    protected override void DrawEditor(int index)
    {
        this.scroll = GUILayout.BeginScrollView(this.scroll);
        {
            base.DrawEditor(index);
        }
        GUILayout.EndScrollView();

        if (this.previewObject)
        {
            GUILayout.FlexibleSpace();
            SirenixEditorGUI.HorizontalLineSeparator(1);
            if (GUILayout.Button("Create Asset", GUILayoutOptions.Height(30)))
            {
                this.CreateAsset();
            }
        }
    }

    private void CreateAsset()
    {
        var t = this.SelectedType;
        if (t == null)
        {
            return;
        }

        string temp = t.text;
        temp = temp.Replace(CLASSNAME, previewObject.fileName);

        string path = targetFolder + "/" + previewObject.fileName + ".cs";
        using(System.IO.StreamWriter sw = new(path,false))
        {
            sw.Write(temp);
        }

        AssetDatabase.Refresh();
        EditorApplication.delayCall += this.Close;
    }
} 
#endif
