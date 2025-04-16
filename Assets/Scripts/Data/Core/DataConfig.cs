#if UNITY_EDITOR
using System.Collections.Generic;
using O2un.Config;
using O2un.Core.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[O2un.Config.GlobalConfig]
public class DataConfig : ConfigHelper<DataConfig>
{
    //[InlineProperty(LabelWidth = 90)]
    [System.Serializable]
    public struct ExcelInfo
    {
        [SerializeField] private string _className;
        [SerializeField] private string _excelPath;
        [SerializeField] private int _sheetIndex;

        public string ClassName => _className;
        public string ExcelPath => _excelPath;
        public int SheetIndex => _sheetIndex;

        public ExcelInfo(string name, string excelpath, int index)
        {
            _className = name;
            _excelPath = excelpath;
            _sheetIndex = index;
        }
        
    }

    [FolderPath]
    public string _dataPath = "Assets/DataTable/.RawData";
    
    [ReadOnly]
    public List<ExcelInfo> DataInfoList = new();

    public void Clear()
    {
        DataInfoList.Clear();
    }

    public bool Add(ExcelInfo value)
    {
        bool rv = DataInfoList.AddUnique(value);
        EditorUtility.SetDirty(Instance);
        
        return rv;
    }

    public ExcelInfo GetPath(string key)
    {
        return DataInfoList.Find((d)=>{
            return d.ClassName == key;
        });
    }
} 
#endif