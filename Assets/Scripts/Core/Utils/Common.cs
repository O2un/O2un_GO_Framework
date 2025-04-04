using UnityEngine;
using System;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace O2un.Core.Utils
{
    public static class CommonUtils
    {
        public static void Exit(int code = 0)
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
        Application.Quit(code);
#endif
        }

        public static object TryParserOrDefault(Type type, string value)
        {
            if (false == type.IsEnum)
            {
                throw new ArgumentException("Provided type must be an Enum.", nameof(type));
            }

            if (Enum.TryParse(type, value, ignoreCase: true, out object result))
            {
                return result;
            }

            return Enum.GetValues(type).Cast<object>().Last();
        }
    }
}
