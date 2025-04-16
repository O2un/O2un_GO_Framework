using GameCommonTypes;

namespace O2un.Data
{
    public abstract class StaticData : object
    {
#if UNITY_EDITOR
        public static readonly string TemplatePath = "Assets/Editor Default Resources/ScriptTemplates/DataScript/StaticData.txt";
        public static readonly string ScriptPath = "Assets/Scripts/Data/StaticData";
        public static readonly string VALUEPREFIX = "//#VALUESTART#";
        public static readonly string VALUESUFFIX = "//#VALUESEND#";
        public static readonly string SUFFIX = "StaticData";
#endif

        public UniqueKey64 Key { get; private set; }

        public void SetKey(uint upperKey, uint lowerKey)
        {
            Key = new(upperKey, lowerKey);
        }

        public void SetKey(ulong key)
        {
            Key = new(key);
        }

        public void SetKey(UniqueKey64 key)
        {
            Key = key;
        }

        public abstract void Set();
        public abstract void Link();
    } 

    public static class StaticDataHelper
    {
        private static readonly string NULL = "<null>";
        public static bool IsNull(this string value)
        {
            return value.Equals(NULL);
        }
    }
}