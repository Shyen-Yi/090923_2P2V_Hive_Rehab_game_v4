using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;

namespace com.hive.projectr
{
    public static class ReflectionUtil
    {
        public static PropertyInfo[] GetPublicInstancePropertiesExcludingObject(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => p.DeclaringType != typeof(object))
                       .ToArray();
        }
    }
}