using UnityEngine;
using System;


#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace O2un.Config
{
#if ODIN_INSPECTOR
    public class GlobalConfig : GlobalConfigAttribute
    {
        public GlobalConfig() : base("Config") {}
    }
#else
    public class GlobalConfig : Attribute
    {

    }
#endif

#if ODIN_INSPECTOR
    public abstract class ConfigHelper<T> : SerializedScriptableObject where T : ConfigHelper<T>, new()
#else
    public abstract class ConfigHelper<T> : ScriptableObject where T : ConfigHelper<T>, new()
#endif
    {
#if ODIN_INSPECTOR
        private static GlobalConfigAttribute configAttribute;
        public static GlobalConfigAttribute ConfigAttribute
        {
            get
            {
                if (configAttribute == null)
                {
                    configAttribute = typeof(T).GetCustomAttribute<GlobalConfigAttribute>();
                    if (configAttribute == null)
                    {
                        configAttribute = new GlobalConfigAttribute(typeof(T).GetNiceName());
                    }
                }

                return configAttribute;
            }
        }
#endif
        private static T _instance;
        public static T Instance
        {
            get
            {
                if(null == _instance)
                {
#if ODIN_INSPECTOR
                    string path = ConfigAttribute.AssetPath + typeof(T).Name;
#else
                    string path = typeof(T).Name;
#endif
                    _instance = Resources.Load<T>(path);
                    if(null == _instance)
                    {
                        T instance = CreateInstance<T>();
                        #if UNITY_EDITOR
                        // 생성은 상대주소
                        path = "Assets/Resources/" + path + ".asset";
                        AssetDatabase.CreateAsset(instance, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        #endif

                        _instance = Resources.Load<T>(path);
                    }
                }

                return _instance;
            }
        }

        public string GetPath()
        {
            if(null == _instance)
            {
                var instance = Instance;
            }

#if ODIN_INSPECTOR
            string path = ConfigAttribute.AssetPath + typeof(T).Name;
#else
            string path = typeof(T).Name;
#endif
            return "Assets/Resources/" + path + ".asset";
        }
    }
}
