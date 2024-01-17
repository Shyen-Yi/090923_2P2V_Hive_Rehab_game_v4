using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.hive.projectr
{
    public class PlayerLevelStorage
    {

    }

    public class DailyProgressData
    {
        public int year;
        public int dayOfYear;
        public DayOfWeek dayOfWeek;
        public int levelCompletionCount;
        public int latestLevelPlayed;
        public int latestLevelPassedStreak;
    }

    public class PlayerProgressManager : SingletonBase<PlayerProgressManager>, ICoreManager
    {
        public int LatestLevelPlayed { get; private set; }
        public int LatestLevelPassedStreak { get; private set; }
        public int DailyLevelCompletion { get; private set; }

        private static readonly string LatestLevelPlayedKey = "LatestLevelPlayed";
        private static readonly string LatestLevelPassedStreakKey = "LatestLevelPassedStreak";
        private static readonly string DailyLevelCompletionKey = "DailyLevelCompletion";

        private bool _needLoadData = true;

        public void OnLevelStarted(int level)
        {
            if (LatestLevelPlayed != level)
            {
                LatestLevelPassedStreak = 0;
                PlayerPrefs.SetInt(LatestLevelPassedStreakKey, 0);
            }

            LatestLevelPlayed = level;
            PlayerPrefs.SetInt(LatestLevelPlayedKey, LatestLevelPlayed);

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
            PlayerPrefs.SetInt(LatestLevelPassedStreakKey, LatestLevelPassedStreak);

            ++DailyLevelCompletion;
            PlayerPrefs.SetInt(DailyLevelCompletionKey, DailyLevelCompletion);

            Debug.LogError($"OnLevelCompleted - LatestLevelPlayed: {LatestLevelPlayed} | LatestLevelPassedStreak: {LatestLevelPassedStreak}");
        }

        public void OnInit()
        {
            if (PlayerPrefs.HasKey(LatestLevelPlayedKey))
            {
                LatestLevelPlayed = PlayerPrefs.GetInt(LatestLevelPlayedKey);
            }
            else
            {
                LatestLevelPlayed = 0;
            }

            if (PlayerPrefs.HasKey(LatestLevelPassedStreakKey))
            {
                LatestLevelPassedStreak = PlayerPrefs.GetInt(LatestLevelPassedStreakKey);
            }
            else
            {
                LatestLevelPassedStreak = 0;
            }

            if (PlayerPrefs.HasKey(DailyLevelCompletionKey))
            {
                DailyLevelCompletion = PlayerPrefs.GetInt(DailyLevelCompletionKey);
            }
            else
            {
                DailyLevelCompletion = 0;
            }
        }

        public void OnDispose()
        {

        }
    }
}