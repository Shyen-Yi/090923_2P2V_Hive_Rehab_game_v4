using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace com.hive.projectr
{
    public static class TypeUtil
    {
        public static string RuntimeAssemblyName = "Assembly-CSharp";
        public static string EditorAssemblyName = "Assembly-CSharp-Editor";

        public static Type GetType(string fullName)
        {
            var output = Type.GetType($"{fullName}, {RuntimeAssemblyName}");
            if (output != null)
            {
                return output;
            }

            output = Type.GetType($"{fullName}, {EditorAssemblyName}");
            if (output != null)
            {
                return output;
            }

            return null;
        }

        public static string GetFriendlyName(this Type type)
        {
            if (!type.IsGenericType)
                return type.Name;

            var sb = new StringBuilder();
            var typeName = type.Name.Substring(0, type.Name.IndexOf('`'));
            sb.Append(typeName);
            sb.Append("<");

            Type[] typeArguments = type.GetGenericArguments();
            for (int i = 0; i < typeArguments.Length; i++)
            {
                sb.Append(GetFriendlyName(typeArguments[i]));
                if (i < typeArguments.Length - 1)
                    sb.Append(", ");
            }

            sb.Append(">");
            return sb.ToString();
        }

        public static string GetFriendlyFullName(this Type type)
        {
            if (!type.IsGenericType)
                return type.FullName;

            var sb = new StringBuilder();
            var typeName = type.FullName.Substring(0, type.FullName.IndexOf('`'));
            sb.Append(typeName);
            sb.Append("<");

            Type[] typeArguments = type.GetGenericArguments();
            for (int i = 0; i < typeArguments.Length; i++)
            {
                sb.Append(GetFriendlyName(typeArguments[i]));
                if (i < typeArguments.Length - 1)
                    sb.Append(", ");
            }

            sb.Append(">");
            return sb.ToString();
        }
    }
}