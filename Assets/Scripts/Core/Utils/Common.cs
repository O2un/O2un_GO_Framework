using UnityEngine;
using System;
using System.Linq;
using Cysharp.Threading.Tasks;



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

        public static int IndexOfEnd(this string str, string find)
        {
            return str.IndexOf(find) + find.Length;
        }

        private static readonly int GLOBAL_DELAY_FRAME = 2;
        public static UniTask DelayFrameAction(Action action)
        {
            return UniTask.DelayFrame(GLOBAL_DELAY_FRAME).ContinueWith(() => { action?.Invoke(); });
        }
    }
}
