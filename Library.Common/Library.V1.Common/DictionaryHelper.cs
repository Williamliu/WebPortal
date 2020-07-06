using System;
using System.Collections.Generic;
using System.Text;

namespace Library.V1.Common
{
    public static class DictionaryHelper
    {
        public static Dictionary<string, object> AddRange(this Dictionary<string, object> dict, Dictionary<string, object> source)
        {
            if (source.Count > 0)
            {
                foreach (string key in source.Keys)
                {
                    dict.Add(key, source[key]);
                }
            }
            return dict;
        }
        public static IDictionary<string, object> AddRange(this IDictionary<string, object> dict, IDictionary<string, object> source)
        {
            if (source.Count > 0)
            {
                foreach (string key in source.Keys)
                {
                    dict.Add(key, source[key]);
                }
            }
            return dict;
        }
    }
}
