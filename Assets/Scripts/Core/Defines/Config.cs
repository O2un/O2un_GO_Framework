using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace O2un.Config
{
    public class GlobalConfig : GlobalConfigAttribute
    {
        public GlobalConfig() : base("Config") {}
    }

    public abstract class ConfigHelper<T> : SerializedScriptableObject where T : ConfigHelper<T>, new()
    {
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

        private static T _instance;
        public static T Instance
        {
            get
            {
                if(null == _instance)
                {
                    string path = ConfigAttribute.AssetPath + typeof(T).Name;
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

            string path = ConfigAttribute.AssetPath + typeof(T).Name;
            return "Assets/Resources/" + path + ".asset";
        }
    }
}
