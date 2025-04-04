using System;
using System.Collections.Generic;

namespace O2un.Core
{
    public static class EnumCache
    {
        private static readonly Dictionary<Enum, string> E_S_CACHE = new();
        public static string ToStringOpt(this Enum value)
        {
            if(false == E_S_CACHE.ContainsKey(value))
            {
                E_S_CACHE.Add(value, value.ToString().Replace("__"," "));
            }
    
            return E_S_CACHE[value];
        }
    }
}