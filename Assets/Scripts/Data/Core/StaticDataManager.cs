using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GameCommonTypes;
using O2un.Core.Excel;
using O2un.Data.Binary;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace O2un.Data
{
    public interface IStaticDataManager
    {
#if UNITY_EDITOR
        public static readonly string TemplatePath = "Assets/Editor Default Resources/ScriptTemplates/DataScript/DataManager.txt";
        public static readonly string ScriptPath = "Assets/Scripts/Data/DataManager";
        public static readonly string SUFFIX = "Manager";
        public static readonly string DATATYPE = "#DATATYPE#";
        public static readonly string FILEPATH = "#FILEPATH#";
#endif
        public static readonly string BINARYPATH = "Assets/DataTable/";
        public static readonly string BINARYSUFFIX = ".txt";
    }

    public abstract class StaticDataManager<M,T> : IStaticDataManager where T : StaticData, new() where M : StaticDataManager<M,T>, new()
    {
        static object _lock = new();
        static private M _instance;
        public static M Instance
        {
            get
            {
                lock(_lock)
                {
                    if(null == _instance)
                    {
                        _instance = new();
                    }
                }
                return _instance;
            }
        }

        protected ImmutableDictionary<UniqueKey64, T> DataList { get; private protected set; }

        public T Get(UniqueKey64 key)
        {
            #if UNITY_EDITOR
            // 유니티 에디터 작업중 데이터가 없으면 로드해서 사용하자
            if(false == Application.isPlaying)
            {
                if(false == Instance.IsLoaded)
                {
                    Excel.RefreshExcelList();
                    Load();
                }
            }
            #endif

            DataList.TryGetValue(key, out T value);
            return value;
        }

        public bool IsExist(UniqueKey64 key)
        {
            return DataList.ContainsKey(key);
        }

        public List<T> ToList()
        {
            return DataList.Values.ToList();
        }

        protected virtual void Clear()
        {
            _isLoaded = false;

            // DataList.Clear();
            // Clear() 대신 새로운 빈 ImmutableDictionary로 변경
            DataList = ImmutableDictionary<UniqueKey64, T>.Empty;
        }

        volatile bool _isLoaded = false;
        public bool IsLoaded => _isLoaded;
        public void Load(bool isLoadFromBinary = false, bool isLoadFromAddressable = false)
        {
            Clear();

            #if UNITY_EDITOR
            // 테스트를 위해 에디터에서는 둘다가능
            // 프로덕트에서는 바이너리만 사용합니다
            if(isLoadFromAddressable)
            {
                LoadFromAddressable();
            }
            else if(isLoadFromBinary)
            {
                LoadFromBinary();
            }
            else
            {
                LoadFromExcel();

                _isLoaded = true;
            }
            #else
            LoadFromAddressable();
            //LoadFromBinary();
            #endif
        }

        protected abstract void LoadFromExcel();

        private static readonly string BINARYPATH = "Assets/DataTable/";
        private static readonly string SUFFIX = ".txt";
        public void SaveToBinary()
        {
            #if UNITY_EDITOR
            var dataType = typeof(T);
            var fieldinfos = dataType.GetRuntimeFields();
            using(var bw = BinaryHelper.SaveToBinary(BINARYPATH+dataType.Name+SUFFIX))
            {
                bw.Write(DataList.Count);
                foreach (var d in DataList)
                {
                    bw.Write(d.Key);
                    foreach (var f in fieldinfos)
                    {
                        bw.Write(f.GetValue(d.Value));
                    }
                }
            }
            #endif
        }

        protected void LoadFromBinary()
        {
            Type type = typeof(T);
            var fieldinfos = type.GetRuntimeFields();

            using (var br = BinaryHelper.LoadFromBinary(BINARYPATH + type.Name + SUFFIX))
            {
                LoadXXX(fieldinfos, br);
            }
        }

        protected void LoadFromAddressable()
        {
            Type type = typeof(T);
            var fieldinfos = type.GetRuntimeFields();

            Addressables.LoadAssetAsync<TextAsset>(BINARYPATH + type.Name + SUFFIX).Completed += LoadCompleted;
        }

        private void LoadCompleted(AsyncOperationHandle<TextAsset> handle)
        {
            Type type = typeof(T);
            var fieldinfos = type.GetRuntimeFields();

            if(AsyncOperationStatus.Succeeded == handle.Status)
            {
                using(var br = BinaryHelper.LoadFromMemory(handle.Result.bytes))
                {
                    LoadXXX(fieldinfos, br);
                }
            }
        }

        private void LoadXXX(IEnumerable<FieldInfo> fieldinfos, BinaryReader br)
        {
            Dictionary<UniqueKey64, T> loadDictionary = new();

            var count = br.ReadInt32();
            for (int ii = 0; ii < count; ++ii)
            {
                T newData = new T();
                newData.SetKey(br.Read<UniqueKey64>());
                foreach (var f in fieldinfos)
                {
                    f.SetValue(newData, br.Read(f.FieldType));
                }

                loadDictionary.Add(newData.Key, newData);
            }

            DataList = loadDictionary.ToImmutableDictionary();

            _isLoaded = true;
        }

        public void Set()
        {
            foreach (var data in DataList)
            {
                data.Value.Set();
            }

            SetXXX();
        }

        protected abstract void SetXXX();
        
        public void Link()
        {
            foreach (var data in DataList)
            {
                data.Value.Link();
            }

            LinkXXX();
        }

        protected abstract void LinkXXX();

        public async Task WaitForLoaded()
        {
            while(false == IsLoaded)
            {
                await Task.Yield();
            }
        }
    } 
}
