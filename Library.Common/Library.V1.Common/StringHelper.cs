using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Library.V1.Common
{
    public static class StringHelper
    {
        public static string Concat(this string str, string joinStr, string space = " ")
        {
            str = str??"";
            str += string.IsNullOrWhiteSpace(str.Trim()) ? joinStr.Trim() : string.IsNullOrWhiteSpace(joinStr.Trim()) ? "" : space + joinStr.Trim();
            return str;
        }

        private static string NewMethod()
        {
            return "";
        }

        public static IList<string> ToStringList(this string str, char spliter = ',')
        {
            IList<string> ret = new List<string>();
            if (!string.IsNullOrWhiteSpace(str))
            {
                ret = str.Split(new char[] { spliter }, StringSplitOptions.RemoveEmptyEntries);
            }
            return ret;
        }
        public static string NL2BR(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return "";
            }
            else
            {
                // SQL string for \n is  \\n  
                return str.Replace("\n", "<br>").Replace("\\n", "<br>").Replace("\r", "<br>").Replace("\\r", "<br>");
            }
        }
        public static string Capital(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return "";
            }
            else
            {
                return str.Substring(0, 1).ToUpper() + str.Substring(1).ToLower();
            }
        }
        public static string Uword(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return "";
            }
            else
            {
                string[] strs = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> rstrs = new List<string>();
                foreach (string s in strs)
                {
                    rstrs.Add(s.Capital());
                }
                return string.Join(" ", rstrs);
            }
        }
        public static string MD5Hash(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return "";
            }
            else
            {
                StringBuilder hash = new StringBuilder();
                MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
                byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(str));

                for (int i = 0; i < bytes.Length; i++)
                {
                    hash.Append(bytes[i].ToString("x2"));
                }
                return hash.ToString();
            }
        }
    }
}
