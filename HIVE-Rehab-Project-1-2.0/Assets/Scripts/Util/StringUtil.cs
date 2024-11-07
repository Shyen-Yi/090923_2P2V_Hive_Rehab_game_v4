using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace com.hive.projectr
{
    public static class StringUtil
    {
        public static string ConvertToCamel(string str)
        {
            var camel = char.ToLowerInvariant(str[0]) + str.Substring(1);
            return camel;
        }

        public static string ConvertToPascal(string str)
        {
            var pascal = char.ToUpperInvariant(str[0]) + str.Substring(1);
            return pascal;
        }
    }
}