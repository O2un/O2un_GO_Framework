#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Text;
using O2un.Core.Excel;
using UnityEditor;
using UnityEngine;
using O2un.Core.Utils;


#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#endif

namespace O2un.Data 
{
    public class DataTool
    {
#if ODIN_INSPECTOR
        [OnInspectorGUI] private void Space1() { GUILayout.Space(20); }
        [ButtonGroup("REGEN")]
        [Button(buttonSize:50, name: "데이터 스크립트 생성")]
#endif
        void DataScriptGenerate()
        {
            Excel.CreateScriptAllData();
        }

        public static readonly string EDITORPATH = "Assets/Scripts/Data/Core/EditorDataLoader.cs";
        public static readonly string RUNTIMEPATH = "Assets/Scripts/Data/Core/RuntimeDataLoader.cs";
        public static readonly string LOAD = "#LOAD#";
        public static readonly string SAVE = "#SAVE#";
        public static readonly string BINARYLOAD = "#BINARYLOADDATA#";
        public static readonly string ADRESSLOAD = "#ADDRESSLOADDATA#";
        public static readonly string LINKING = "#LINK#";
        public static readonly string WAIT = "#WAIT#";

        public static readonly string editorTemplate = "Assets/Editor Default Resources/ScriptTemplates/DataScript/EditorDataLoader.txt";
#if ODIN_INSPECTOR
        [ButtonGroup("REGEN")]
        [Button(buttonSize: 50 , name: "데이터 로드 스크립트 생성")]
#endif
        private void EditorLoaderScriptGenerate()
        {
            string template;
            using (StreamReader sr = new(editorTemplate))
            {
                template = sr.ReadToEnd();
            }
            
            var temp = CommonUtils.GetTypes(t => t.IsClass && typeof(IStaticDataManager).IsAssignableFrom(t) && !t.IsAbstract);
    
            StringBuilder values = new();
            StringBuilder values2 = new();
            StringBuilder values3_0 = new();
            StringBuilder values3_1 = new();

            foreach (var m in temp)
            {
                values.AppendLine($"\t\t\t{m.Name}.Instance.Load();");
                values2.AppendLine($"\t\t\t{m.Name}.Instance.SaveToBinary();");
                values3_0.AppendLine($"\t\t\t{m.Name}.Instance.Set();");
                values3_1.AppendLine($"\t\t\t{m.Name}.Instance.Link();");
            };
            //스트링 테이블
            // values.AppendLine($"\t\t\tStringTableManager.Instance.LoadFromExcel();");
            // values2.AppendLine($"\t\t\tStringTableManager.Instance.SaveToBinary();");

            template = template.Replace(LOAD, values.ToString());
            template = template.Replace(SAVE, values2.ToString());
            template = template.Replace(LINKING, values3_0.ToString() + "\n" + values3_1.ToString());
    
            using (StreamWriter sw = new(EDITORPATH, false))
            {
                sw.Write(template);
            }

            RuntimeLoaderScriptGenerate();
        }
        
        public static readonly string TemplatePath = "Assets/Editor Default Resources/ScriptTemplates/DataScript/RuntimeDataLoader.txt";
        private void RuntimeLoaderScriptGenerate()
        {
            string template;
            using (StreamReader sr = new(TemplatePath))
            {
                template = sr.ReadToEnd();
            }
            
            var temp = CommonUtils.GetTypes(t => t.IsClass && typeof(IStaticDataManager).IsAssignableFrom(t) && !t.IsAbstract);
    
            StringBuilder values = new();
            StringBuilder values2 = new();
            StringBuilder values3_0 = new();
            StringBuilder values3_1 = new();
            StringBuilder values4 = new();
            
            foreach (var m in temp)
            {
                values.AppendLine($"\t\t\t{m.Name}.Instance.Load(true);");
                values2.AppendLine($"\t\t\t{m.Name}.Instance.Load(true, true);");
                values3_0.AppendLine($"\t\t\t{m.Name}.Instance.Set();");
                values3_1.AppendLine($"\t\t\t{m.Name}.Instance.Link();");
                values4.AppendLine($"\t\t\t\t{m.Name}.Instance.WaitForLoaded(),");
            };
            values4.Replace(',',' ',values4.Length-5,4);

            // values.AppendLine($"\t\t\tStringTableManager.Instance.Load(true);");
            // values2.AppendLine($"\t\t\tStringTableManager.Instance.Load(true, true);");

            template = template.Replace(BINARYLOAD, values.ToString());
            template = template.Replace(ADRESSLOAD, values2.ToString());
            template = template.Replace(LINKING, values3_0.ToString() + "\n" + values3_1.ToString());
            template = template.Replace(WAIT, values4.ToString());
            
            using (StreamWriter sw = new(RUNTIMEPATH, false))
            {
                sw.Write(template);
            }

            AssetDatabase.Refresh();
        }

#if ODIN_INSPECTOR
        [ButtonGroup("REGEN")]
        [Button(buttonSize:50, name: "엑셀 리스트 리로드")]
#endif
        void ExcelListRefesh()
        {
            Excel.RefreshExcelList(true);
        }

#if ODIN_INSPECTOR
        [OnInspectorGUI] private void Space2() { GUILayout.Space(20); }
        [ButtonGroup("LOAD")]
        [Button(buttonSize: 50 , name: "엑셀 데이터 로드")]
#endif
        private void EditorDataLoadTest()
        {
            EditorDataLoader.LoadFromExcel();
        }

#if ODIN_INSPECTOR
        [ButtonGroup("LOAD")]
        [Button(buttonSize: 50 , name: "바이너리 데이터 로드")]
#endif
        private void RuntimeDataLoadTest()
        {
            RuntimeDataLoader.LoadFromBinary();
        }

#if ODIN_INSPECTOR
        [ButtonGroup("LOAD")]
        [Button(buttonSize: 50 , name: "어드레서블 데이터 로드")]
#endif
        private void AddressableLoad()
        {
            RuntimeDataLoader.LoadFromAddressable();
        }

#if ODIN_INSPECTOR
        [OnInspectorGUI] private void Space3() { GUILayout.Space(20); }
        [ButtonGroup("SAVE")]
        [Button(buttonSize: 50 , name: "바이너리 저장")]
#endif
        private void SaveToBinary()
        {
            EditorDataLoader.SaveToBinary();
        }

#if ODIN_INSPECTOR
        [OnInspectorGUI] private void Space4() { GUILayout.Space(20); }
        [ButtonGroup("PlayerPrefab")]
        [Button(buttonSize: 50 , name: "Player Prefab 초기화")]
#endif
        private void ClearPlayerPrefab()
        {
            PlayerPrefs.DeleteAll();
        }
    } 
} 
#endif
