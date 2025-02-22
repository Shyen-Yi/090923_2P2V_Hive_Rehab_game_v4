using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    /// @ingroup GameCommon
    /// @class PlayerPrefKeys
    /// @brief Holds the keys for accessing player preferences stored in Unity's PlayerPrefs.
    /// 
    /// The `PlayerPrefKeys` class is a central location for storing the keys used to access various player preference data.
    /// This includes settings like the display name, level information, player performance, and stats that are stored in Unity's
    /// `PlayerPrefs` system.
    public class PlayerPrefKeys
    {
        // level
        /// <summary>
        /// The key used to store the current winning streak for the current level played by the player.
        /// </summary>
        public static readonly string LevelStreakData = "LevelStreakData";

        // setting
        /// <summary>
        /// The key used to store the player's display name.
        /// </summary>
        public static readonly string DisplayName = "DisplayName";
        /// <summary>
        /// The key used to store the LevelTotal (total number of asteroids to spawn in each level).
        /// </summary>
        public static readonly string LevelTotal = "LevelTotal";
        /// <summary>
        /// The key used to store the LevelGoal (number of asteroids to collect to win a level).
        /// </summary>
        public static readonly string LevelGoal = "LevelGoal";

        // stats
        /// <summary>
        /// The key used to store the days the player has played.
        /// </summary>
        public static readonly string PlayedDays = "PlayedDays";
        /// <summary>
        /// The key used to store the player's last performance.
        /// </summary>
        public static readonly string LastPerformance = "LastPerformance";
        /// <summary>
        /// The key used to store the description of the player's last performance.
        /// </summary>
        public static readonly string LastPerformanceDesc = "LastPerformanceDesc";
    }
}