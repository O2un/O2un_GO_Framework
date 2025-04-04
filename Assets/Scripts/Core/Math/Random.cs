using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace O2un.Core
{
    public static class Random
    {
        public static bool Draw(this int value, out int result)
        {
            result = UnityEngine.Random.Range(0, Math.C100PERCENT);
            return result <= value;
        }

        public static Vector3 GetRandomPosition(this Vector3 origin, float radius)
        {
            return new Vector3(origin.x + UnityEngine.Random.Range(-radius,radius) ,origin.y,origin.z + UnityEngine.Random.Range(-radius,radius));
        }

        public static T PickOne<T>(this List<T> list)
        {
            var len = list.Count;
            if(0 == len)
            {
                return default;
            }

            return list[UnityEngine.Random.Range(0, len)];
        }

        public static List<T> PickCount<T>(this List<T> list, int count)
        {
            List<T> pick = new();
            if(count <= 0)
            {
                return pick;
            }
            else if(count <= list.Count)
            {
                pick = list;
            }
            else
            {
                var shuffled = list.Shuffle();
                pick.AddRange(shuffled.GetRange(0, count));
            }
            return pick;
        }

        public static List<T> Shuffle<T>(this List<T> list)
        {
            System.Random rand = new();
            return list.OrderBy(_=> rand.Next()).ToList();;
        }
    }
}
