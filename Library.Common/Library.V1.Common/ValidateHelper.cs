using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Library.V1.Common
{
    public static class ValidateHelper
    {
        public static bool IsMatch(string dataType, object value, object value1 = null)
        {
            if (ObjectHelper.IsEmpty(value) && ObjectHelper.IsEmpty(value1)) return true;
            string pattern = string.Empty;
            switch (dataType)
            {
                case "Hidden":
                case "Object":
                case "String":
                    pattern = DataType["all"];
                    break;
                case "Email":
                    pattern = DataType["email"];
                    break;
                case "Int":
                case "Long":
                case "Float":
                    pattern = DataType["number"];
                    break;
                case "Date":
                    pattern = DataType["date"];
                    break;
                case "DateTime":
                    pattern = DataType["datetime"];
                    break;
                case "Time":
                    pattern = DataType["time"];
                    break;
                case "Password":
                case "Passpair":
                    pattern = DataType["password"];
                    break;
                case "Bool":
                case "Scan":
                case "Checkbox":
                    break;
            }

            bool flag = true;
            if (string.IsNullOrWhiteSpace(pattern) == false)
            {
                if (string.IsNullOrWhiteSpace(value.GetString()) == false)
                {
                    Regex regex = new Regex(pattern);
                    Match match = regex.Match(value.GetString());
                    if (match.Success)
                        flag = flag && true;
                    else
                        flag = flag && false;
                }
                if (string.IsNullOrWhiteSpace(value1.GetString()) == false)
                {
                    Regex regex = new Regex(pattern);
                    Match match = regex.Match(value1.GetString());
                    if (match.Success)
                        flag = flag && true;
                    else
                        flag = flag && false;
                }
            }
            return flag;
        }

        private static Dictionary<string, string> DataType = new Dictionary<string, string> {
            {"all", @".*"},
            {"phone", @".*" },
            {"password", @".*"},
            {"number", @"^(\+|-)?[0-9]+[.]?[0-9]*$"},
            {"email", @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$" },
            {"date", @"^(?:19|20)[0-9]{2}(?:-|\/)(?:1[0-2]|0?[1-9])(?:-|\/)(?:3[01]|[0-2]?[0-9])$" },
            {"datetime", @"^(?:19|20)[0-9]{2}(?:-|\/)(?:1[0-2]|0?[1-9])(?:-|\/)(?:3[01]|[0-2]?[0-9])[ ]+((2[0-3]|[01]?[0-9])(:[0-5][0-9](:[0-5][0-9])?)?[ ]*(am|pm)?)$" },
            {"time", @"^((2[0-3]|[01]?[0-9])(:[0-5]?[0-9](:[0-5]?[0-9])?)?[ ]*(am|pm)?)$"}
        };
    }
}
