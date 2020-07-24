using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.V1.Common
{
    public static class ObjectHelper
    {
        private static IDictionary<Type, string> DataTypeList = new Dictionary<Type, string>();
        static ObjectHelper()
        {
            DataTypeList.Add(typeof(object), "Object");
            DataTypeList.Add(typeof(string), "String");
            DataTypeList.Add(typeof(byte), "Byte");
            DataTypeList.Add(typeof(short), "Short");
            DataTypeList.Add(typeof(int), "Int");
            DataTypeList.Add(typeof(long), "Long");
            DataTypeList.Add(typeof(double), "Double");
            DataTypeList.Add(typeof(decimal), "Decimal");
            DataTypeList.Add(typeof(float), "Float");
            DataTypeList.Add(typeof(bool), "Bool");
            DataTypeList.Add(typeof(DateTime), "DateTime");
            DataTypeList.Add(typeof(TimeSpan), "TimeSpan");
            DataTypeList.Add(typeof(Dictionary<string, bool>), "Checkbox");
        }
        public static string GetTypeString(this object obj)
        {
            if (obj != null)
                if (DataTypeList.ContainsKey(obj.GetType()))
                    return DataTypeList[obj.GetType()];
                else
                    return "";
            else
                return "Null";
        }

        public static T GetValue<T>(this object val)
        {
            T t = default;
            try
            {
                t = (T)val;
            }
            catch
            {
            }
            return t;
        }

        #region Get Methods
        public static bool IsEmpty(this object val)
        {
            return string.IsNullOrWhiteSpace(val.GetString());
        }

        public static string GetString(this object val)
        {
            string nv = string.Empty;
            switch (val.GetTypeString())
            {
                case "Null":
                case "Object":
                case "String":
                    nv = val.GetValue<string>() ?? "";
                    break;
                case "Byte":
                    nv = val.GetValue<byte>().ToString();
                    break;
                case "Short":
                    nv = val.GetValue<short>().ToString();
                    break;
                case "Int":
                    nv = val.GetValue<int>().ToString();
                    break;
                case "Long":
                    nv = val.GetValue<long>().ToString();
                    break;
                case "Double":
                    nv = val.GetValue<double>().ToString();
                    break;
                case "Decimal":
                    nv = val.GetValue<decimal>().ToString();
                    break;
                case "Float":
                    nv = val.GetValue<float>().ToString();
                    break;
                case "Bool":
                    nv = val.GetValue<bool>().ToString();
                    break;
                case "DateTime":
                    nv = val.GetValue<DateTime>() == DateTime.MinValue ? "" : val.GetValue<DateTime>().ToString();
                    break;
                case "TimeSpan":
                    nv = val.GetValue<TimeSpan>() == TimeSpan.MinValue ? "" : val.GetValue<TimeSpan>().ToString();
                    break;
                default:
                    break;
            }
            nv = nv.Trim();
            return nv;
        }
        public static byte? GetByte(this object val)
        {
            try
            {
                float nv = default;
                if (float.TryParse(val.GetString(), out nv))
                    return (byte)nv;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public static short? GetShort(this object val)
        {
            try
            {
                float nv = default;
                if (float.TryParse(val.GetString(), out nv))
                    return (short)nv;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public static int? GetInt(this object val)
        {
            try
            {
                if (val.GetType().ToString() == "System.Boolean")
                {
                    bool ok = (bool)val;
                    if (ok) 
                        return 1;
                    else
                        return 0;
                }

                float nv = default;
                if (float.TryParse(val.GetString(), out nv))
                    return (int)nv;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public static long? GetLong(this object val)
        {
            try
            {
                float nv = default;
                if (float.TryParse(val.GetString(), out nv))
                    return (long)nv;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public static double? GetDouble(this object val)
        {
            try
            {
                float nv = default;
                if (float.TryParse(val.GetString(), out nv))
                    return (double)nv;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public static decimal? GetDecimal(this object val)
        {
            try
            {
                float nv = default;
                if (float.TryParse(val.GetString(), out nv))
                    return (decimal)nv;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public static float? GetFloat(this object val)
        {
            try
            {
                float nv = default;
                if (float.TryParse(val.GetString(), out nv))
                    return nv;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public static bool? GetBool(this object val)
        {
            try
            {
                bool nv = default;
                if (bool.TryParse(val.GetString(), out nv))  //""=false; "ABC" = false; "TRUE" = true; "FALSE"= false
                {
                    return nv;
                }
                else
                {
                    int tint = default;
                    if (int.TryParse(val.GetString(), out tint)) if (tint > 0) return true;

                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        public static DateTime? GetDateTime(this object val)
        {
            try
            {
                DateTime t = default;
                if (DateTime.TryParse(val.GetString(), out t))
                {
                    // "23:30:30"  => Today's Date + TimeSpan  "2020-06-26 23:23:30"
                    if (t < new DateTime(1800, 1, 1)) t = new DateTime(1800, 1, 1);
                    return t;
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public static string GetDateYMDHMS(this object val)
        {
            return val.GetDateTime()?.YMDHMS() ?? "";
        }
        public static string GetDateYMDHM(this object val)
        {
            return val.GetDateTime()?.YMDHM() ?? "";
        }
        public static string GetDate(this object val)
        {
            return val.GetDateTime()?.YMD() ?? "";
        }
        public static TimeSpan? GetTimeSpan(this object val)
        {
            try
            {
                TimeSpan t = default;
                if (TimeSpan.TryParse(val.GetString(), out t))
                    return t;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public static string GetTime(this object val)
        {
            return val.GetDateTime()?.HM() ?? val.GetTimeSpan()?.HM();
        }
        public static string GetTimeHMS(this object val)
        {
            return val.GetDateTime()?.HMS() ?? val.GetTimeSpan()?.HMS();
        }
        public static Dictionary<string, bool> GetCheckbox(this object obj)
        {
          
            Dictionary<string, bool> ck = new Dictionary<string, bool>();
            try
            {
                JObject jo = JObject.Parse(obj.ToString());
                ck = jo.ToObject<Dictionary<string, bool>>();
                if (ck == null) ck = new Dictionary<string, bool>();
            }
            catch { }
            return ck;
        }
        #endregion
    }
}
