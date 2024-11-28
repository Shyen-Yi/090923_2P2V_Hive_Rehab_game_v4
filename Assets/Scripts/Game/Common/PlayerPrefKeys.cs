using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class PlayerPrefKeys
    {
        // level
        public static readonly string LatestLevelPlayed = "LatestLevelPlayed";
        public static readonly string LevelStreaks = "LevelStreaks";

        // setting
        public static readonly string DisplayName = "DisplayName";
        public static readonly string LevelTotal = "LevelTotal";
        public static readonly string LevelGoal = "LevelGoal";

        // stats
        public static readonly string PlayedDays = "PlayedDays";
        public static readonly string LastPerformance = "LastPerformance";
        public static readonly string LastPerformanceDesc = "LastPerformanceDesc";
    }
}