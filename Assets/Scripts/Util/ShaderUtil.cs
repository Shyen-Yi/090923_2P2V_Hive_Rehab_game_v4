using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class ShaderUtil
    {
        public static void SetRadialFill(Material mat, float fill)
        {
            mat.SetFloat("_Arc2", 360 - 360 * fill);
        }
    }
}