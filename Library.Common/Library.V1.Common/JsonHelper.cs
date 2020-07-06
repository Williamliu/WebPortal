using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.V1.Common
{
    public static class JsonExtennsions
    {
        public static T Path<T>(this JObject jo, string jpath)
        {
            T t = default(T);
            try
            {
                if (jo.SelectToken(jpath) != null) t = jo.SelectToken(jpath).Value<T>();
            }
            catch
            {
            }
            return t;
        }
        public static T Path<T>(this JToken jtoken, string jpath)
        {
            T t = default(T);
            try
            {
                if (jtoken.SelectToken(jpath) != null) jtoken.SelectToken(jpath).Value<T>();
            }
            catch
            {
            }
            return t;
        }

        public static IList<T> Array<T>(this JObject jo, string jpath)
        {
            IList<T> t = new List<T>();
            try
            {
                t = jo.SelectTokens(jpath).Values<T>().ToList();
            }
            catch
            {
            }
            return t;
        }
        public static IList<T> Array<T>(this JToken jt, string jpath)
        {
            IList<T> t = new List<T>();
            try
            {
                t = jt.SelectTokens(jpath).Values<T>().ToList();
            }
            catch
            {
            }
            return t;
        }
    }
}
