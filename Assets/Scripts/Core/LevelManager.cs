using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class LevelManager : SingletonBase<LevelManager>, ICoreManager
    {
        public int LatestLevelPlayed { get; private set; }
        public int LatestLevelPassedStreak { get; private set; }

        public void OnLevelStarted(int level)
        {
            if (LatestLevelPlayed != level)
            {
                LatestLevelPassedStreak = 0;
                PlayerPrefs.SetInt(PlayerPrefKeys.LatestLevelPassedStreak, 0);
            }

            LatestLevelPlayed = level;
            PlayerPrefs.SetInt(PlayerPrefKeys.LatestLevelPlayed, LatestLevelPlayed);

            Debug.LogError($"OnLevelStarted - LatestLevelPlayed: {LatestLevelPlayed} | LatestLevelPassedStreak: {LatestLevelPassedStreak}");
        }

        public void OnLevelCompleted(int level, bool isPassed)
        {
            if (level != LatestLevelPlayed)
            {
                Debug.LogError($"Level completed {level} doesn't match the level started {LatestLevelPlayed}!");
                return;
            }

            // save latest level result
            LatestLevelPassedStreak = isPassed ? LatestLevelPassedStreak + 1 : 0;
            PlayerPrefs.SetInt(PlayerPrefKeys.LatestLevelPassedStreak, LatestLevelPassedStreak);

            Debug.LogError($"OnLevelCompleted - LatestLevelPlayed: {LatestLevelPlayed} | LatestLevelPassedStreak: {LatestLevelPassedStreak}");
        }

        public void OnInit()
        {
            if (PlayerPrefs.HasKey(PlayerPrefKeys.LatestLevelPlayed))
            {
                LatestLevelPlayed = PlayerPrefs.GetInt(PlayerPrefKeys.LatestLevelPlayed);
            }
            else
            {
                LatestLevelPlayed = 0;
            }

            if (PlayerPrefs.HasKey(PlayerPrefKeys.LatestLevelPassedStreak))
            {
                LatestLevelPassedStreak = PlayerPrefs.GetInt(PlayerPrefKeys.LatestLevelPassedStreak);
            }
            else
            {
                LatestLevelPassedStreak = 0;
            }
        }

        public void OnDispose()
        {

        }
    }
}