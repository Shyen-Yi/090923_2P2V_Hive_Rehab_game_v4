using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    /// <summary>
    /// @ingroup Utility
    /// @class ShaderUtil
    /// @brief A utility class for general shader-related operations.
    /// </summary>
    public class ShaderUtil
    {
        /// <summary>
        /// Sets the radial fill amount for a 'RadialFillMat' material, adjusting the "_Arc2" property.
        /// </summary>
        /// <param name="mat">The material instance to modify.</param>
        /// <param name="fill">The fill amount, represented as a float between 0 and 1.</param>
        /// <remarks>
        /// The value of 'fill' is used to calculate the angle for the radial fill. A fill of 1.0 will set the angle to 0, and 0.0 will set it to 360.
        /// </remarks>
        public static void SetRadialFill(Material mat, float fill)
        {
            mat.SetFloat("_Arc2", 360 - 360 * fill);
        }
    }
}