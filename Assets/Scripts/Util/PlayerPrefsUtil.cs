using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public static class PlayerPrefsUtil
    {
        public static bool IsValidKey(string key)
        {
            return !string.IsNullOrEmpty(key);
        }

        public static bool HasKey(string key)
        {
            if (!IsValidKey(key))
            {
                return false;
            }

            return PlayerPrefs.HasKey(key);
        }

        public static int GetInt(string key)
        {
            if (HasKey(key))
            {
                return PlayerPrefs.GetInt(key);
            }

            Logger.LogError($"GetInt::Invalid key: {key}");
            return 0;
        }

        public static string GetString(string key)
        {
            if (HasKey(key))
            {
                return PlayerPrefs.GetString(key);
            }

            Logger.LogError($"GetString::Invalid key: {key}");
            return null;
        }

        public static bool TrySetInt(string key, int value)
        {
            if (IsValidKey(key))
            {
                PlayerPrefs.SetInt(key, value);
                return true;
            }

            return false;
        }

        public static bool TrySetString(string key, string value)
        {
            if (IsValidKey(key))
            {
                PlayerPrefs.SetString(key, value);
                return true;
            }

            return false;
        }

        public static bool TryDeleteKey(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
                return true;
            }

            return false;
        }
    }
}