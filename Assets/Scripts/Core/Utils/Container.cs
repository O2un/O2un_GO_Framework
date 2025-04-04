using System.Collections.Generic;
using UnityEngine;

namespace O2un.Core.Utils
{
    public static class ContainerUtils
    {
        public static void AddList<K, V>(this Dictionary<K, List<V>> dic, K key, V value)
        {
            if (dic.TryGetValue(key, out var list))
            {
                list.Add(value);
            }
            else
            {
                dic.TryAdd(key, new() { value });
            }
        }

        public static void AddOrSwap<K, V>(this Dictionary<K, V> dic, K key, V value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = value;
            }
            else
            {
                dic.TryAdd(key, value);
            }
        }

        public static bool AddUnique<T>(this List<T> list, T elemnet)
        {
            if (false == list.Contains(elemnet))
            {
                list.Add(elemnet);

                return true;
            }

            return false;
        }

        public static void Vary<K>(this Dictionary<K, int> dic, K key, int value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] += value;
            }
            else
            {
                dic.TryAdd(key, value);
            }
        }
    }
}
