using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    /// <summary>
    /// @ingroup Utility
    /// @class UIUtil
    /// @brief A utility class for in-game UI calculations.
    /// </summary>
    public class UIUtil
    {
        /// <summary>
        /// Converts a screen position to the game coordinate (with origin at the center of screen).
        /// </summary>
        /// <param name="screenPos">The screen position to convert.</param>
        /// <returns>A Vector2 representing the game coordinate corresponding to the input screen position.</returns>
        public static Vector2 ScreenPosToGameCoordinate(Vector2 screenPos)
        {
            return new Vector2(screenPos.x - Screen.width / 2, screenPos.y - Screen.height / 2);
        }

        /// <summary>
        /// Converts a world position to the game coordinate (with origin at the center of screen).
        /// </summary>
        /// <param name="worldPos">The world position to convert.</param>
        /// <returns>A Vector2 representing the game coordinate corresponding to the input world position.</returns>
        public static Vector2 WorldPosToGameCoordinate(Vector3 worldPos)
        {
            var screenPos = CameraManager.Instance.MainCamera.WorldToScreenPoint(worldPos);
            return ScreenPosToGameCoordinate(screenPos);
        }
    }
}