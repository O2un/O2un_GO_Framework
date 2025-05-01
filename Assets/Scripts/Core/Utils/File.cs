using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace O2un.Core.Utils
{
    public static class FileUtils
    {
        private static readonly string ASSETS = "Assets/";

        public static string ToRelativePath(this string path)
        {
            return path.Substring(path.IndexOf(ASSETS));
        }

#if UNITY_EDITOR
        public static List<T> LoadAllAssetInFolder<T>(string[] path) where T : UnityEngine.Object
        {
            List<T> list = new();
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", path);
            foreach (var guid in guids)
            {
                list.Add((T)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(T)));
            };

            return list;
        }

        public static List<T> GetSubObjects<T>(this UnityEngine.Object asset) where T : UnityEngine.Object
        {
            List<T> list = new();

            var objs = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(asset));
            foreach (var o in objs)
            {
                if (o is T)
                {
                    list.Add(o as T);
                }
            };

            return list;
        }
#endif

        public static string FormatBytes(this long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }
    }
}
