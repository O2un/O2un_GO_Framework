using System;
using System.Collections.Generic;
using UnityEngine;

namespace O2un.Core
{
    public static class TagLayerManager
    {
        public static bool IsTag(this Transform transform, UnityTags tag)
        {
            return transform.CompareTag(tag.ToStringOpt());
        }

        private static readonly Dictionary<string, UnityTags> S_T_Dictionary = new();
        public static UnityTags GetTag(this Transform transform)
        {
            string key = transform.tag;
            if (false == S_T_Dictionary.TryGetValue(key, out UnityTags tag))
            {
                Enum.TryParse(transform.tag, out UnityTags parse);
                S_T_Dictionary.Add(key, parse);
                tag = parse;
            }

            return tag;
        }

        public static LayerMask MakeLayerMask(params UnityLayers[] layers)
        {
            LayerMask result = 0;
            foreach (var l in layers)
            {
                result |= LayerMask.GetMask(l.ToStringOpt());
            }

            return result;
        }
    }
}
