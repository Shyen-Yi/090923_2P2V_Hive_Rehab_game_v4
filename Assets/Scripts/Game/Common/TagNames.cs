using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    /// @ingroup GameCommon
    /// @class TagNames
    /// @brief A collection of constant tag names used throughout the game for tagging game objects in Unity.
    /// 
    /// The `TagNames` class contains static string fields for various tags used in the game. These tags are typically assigned to
    /// GameObjects in Unity to help identify and organize objects within the scene. The class provides a central place to define
    /// all tag names, ensuring consistency across the game and making it easier to modify tags in one location.
    public static class TagNames
    {
        public static string VacuumAir = "VacuumAir";
        public static string VacuumBase = "VacuumBase";
        public static string Spacecraft = "Spacecraft";
        public static string MainCamera = "MainCamera";
        public static string UICamera = "UICamera";
        public static string Asteroid = "Asteroid";
        public static string AudioSourceRoot = "AudioSourceRoot";
        public static string MotionTracker = "MotionTracker";
        public static string Cursor = "Cursor";
    }
}