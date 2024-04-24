using System.Collections;
using System.IO;
using UnityEngine;
using System.Text;
using System;
using System.Collections.Generic;

namespace com.hive.projectr
{
    #region CSV Data
    public struct CSVCoreGameEndedData
    {
        public int level;
        public int capturedCount;
        public int collectedCount;
        public int totalCount;
        public int successRate; // percentage

        public CSVCoreGameEndedData(int level, int capturedCount, int collectedCount, int totalCount, int successRate)
        {
            this.level = level;
            this.capturedCount = capturedCount;
            this.collectedCount = collectedCount;
            this.totalCount = totalCount;
            this.successRate = successRate;
        }
    }

    public struct CSVCoreGameTickData
    {
        public float coreGameTime;
        public Vector2 cursorCoordinate;

        public CSVCoreGameTickData(float coreGameTime, Vector2 cursorCoordinate)
        {
            this.coreGameTime = coreGameTime;
            this.cursorCoordinate = cursorCoordinate;
        }
    }

    public struct CSVCoreGameAsteroidEndedData
    {
        public int asteroidId;

        // spawn
        public float asteroidSpawnTime;
        public Vector2 asteroidCoordinateWhenSpawned;
        public Vector2 cursorCoordinateWhenAsteroidSpawned;

        // capture
        public bool isAsteroidCaptured;
        public float timeSpentToCaptureAsteroid;
        public Vector2 asteroidCoordinateWhenCaptured;
        public Vector2 cursorCoordinateWhenAsteroidCaptured;

        // collect
        public bool isAsteroidCollected;
        public float timeSpentToCollectAsteroid;
        public Vector2 asteroidCoordinateWhenCollected;
        public Vector2 vacuumCenterCoordinateWhenCollected;

        public CSVCoreGameAsteroidEndedData(int asteroidId, float asteroidSpawnTime, Vector2 asteroidCoordinateWhenSpawned, Vector2 cursorCoordinateWhenAsteroidSpawned, bool isAsteroidCaptured, float timeSpentToCaptureAsteroid, Vector2 asteroidCoordinateWhenCaptured, Vector2 cursorCoordinateWhenAsteroidCaptured, bool isAsteroidCollected, float timeSpentToCollectAsteroid, Vector2 asteroidCoordinateWhenCollected, Vector2 vacuumCenterCoordinateWhenCollected)
        {
            this.asteroidId = asteroidId;
            this.asteroidSpawnTime = asteroidSpawnTime;
            this.asteroidCoordinateWhenSpawned = asteroidCoordinateWhenSpawned;
            this.cursorCoordinateWhenAsteroidSpawned = cursorCoordinateWhenAsteroidSpawned;
            this.isAsteroidCaptured = isAsteroidCaptured;
            this.timeSpentToCaptureAsteroid = timeSpentToCaptureAsteroid;
            this.asteroidCoordinateWhenCaptured = asteroidCoordinateWhenCaptured;
            this.cursorCoordinateWhenAsteroidCaptured = cursorCoordinateWhenAsteroidCaptured;
            this.isAsteroidCollected = isAsteroidCollected;
            this.timeSpentToCollectAsteroid = timeSpentToCollectAsteroid;
            this.asteroidCoordinateWhenCollected = asteroidCoordinateWhenCollected;
            this.vacuumCenterCoordinateWhenCollected = vacuumCenterCoordinateWhenCollected;
        }
    }

    public struct CSVCalibrationEndedData
    {
        public Vector2 center;
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomRight;
        public Vector2 bottomLeft;

        public CSVCalibrationEndedData(Vector2 center, Vector2 topLeft, Vector2 topRight, Vector2 bottomRight, Vector2 bottomLeft)
        {
            this.center = center;
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
            this.bottomLeft = bottomLeft;
        }
    }
    #endregion

    public enum CSVType
    {
        CoordinatePos,
        Summary,
        Coordinates,
    }

    public class CSVManager : SingletonBase<CSVManager>, ICoreManager
    {
        private bool _isLogging;

        private int _calibrationNum;
        private int _coreGameNum;
        private int _sessionNum;
        private DateTime _logTime;
        private Dictionary<CSVType, StringBuilder> _logSbDict = new Dictionary<CSVType, StringBuilder>();

        private string _coordinatePosFilePath;
        private string _coordinatesFilePath;
        private string _summaryFilePath;

        private static readonly string DayFolderTemplate = "{0:00}/{1:00}/{2:0000}.{3}"; // 03/13/2024.Mark
        private static readonly string SessionFolderTemplate = SessionNumPrefix + "{0}.{1:00}:{2:00}"; // Session#3.14:25
        private static readonly string SessionNumPrefix = "Session#";
        private static readonly string CoordinatePosFileTemplate = "{0}_{1}/{2}/{3}_{4}Block_Level{5}_CoordinatePOS"; // Mark_03/13/2024_100Block_Level1_CoordinatePOS
        private static readonly string CoordinatesFileTemplate = "{0}_{1}/{2}/{3}_{4}Block_Level{5}_Coordinates"; // Mark_03/13/2024_100Block_Level1_Coordinates
        private static readonly string SummaryFileTemplate = "{0}_Summary"; // Mark_Summary

        #region Lifecycle
        public void OnInit()
        {
            SetupDayFolder(TimeUtil.Now);

            MonoBehaviourUtil.OnApplicationQuitEvent += OnApplicationQuit;
        }

        public void OnDispose()
        {
            MonoBehaviourUtil.OnApplicationQuitEvent -= OnApplicationQuit;
        }
        #endregion

        #region Event
        private void OnApplicationQuit()
        {
            TryEndLog();
        }
        #endregion

        #region Calibration
        public void OnCalibrationStarted(DateTime logTime)
        {
            StartLog(logTime);

            // coordinatePos
            AppendLog(CSVType.CoordinatePos, $"Calibration #{_calibrationNum}\n");
            AppendLog(CSVType.CoordinatePos, $"Centre Point X, Centre Point Y, Top Left X, Top Left Y, Top Right X, Top Right Y, Bottom Right X, Bottom Right Y, Bottom Left X, Bottom Left Y\n");
        }

        /// <summary>
        /// Called when a calibration is finished.
        /// Save calibration CSV data.
        /// </summary>
        /// <param name="data"></param>
        public void OnCalibrationEnded(CSVCalibrationEndedData data)
        {
            // coordinatePos
            AppendLog(CSVType.CoordinatePos, $"{data.center.x}, {data.center.y}, {data.topLeft.x}, {data.topLeft.y}, {data.topRight.x}, {data.topRight.y}, {data.bottomRight.x}, {data.bottomRight.y}, {data.bottomLeft.x}, {data.bottomLeft.y}\n");

            TryEndLog();
        }
        #endregion

        #region Core Game
        public void OnCoreGameStarted(DateTime logTime)
        {
            StartLog(logTime);

            // coordinatePos
            AppendLog(CSVType.CoordinatePos, $"Exercise #{_coreGameNum}");
            AppendLog(CSVType.CoordinatePos, $"Asteroid ID, Asteroid Spawn Time, Asteroid's Coordinate X (Spawned), Asteroid's Coordinate Y (Spawned), Cursor's Coordinate X (Spawned), Cursor's Coordinate Y (Spawned), Is Asteroid Captured, Asteroid Capture Time, Asteroid's Coordinate X (Captured), Asteroid's Coordinate Y (Captured), Cursor's Coordinate X (Captured), Cursor's Coordinate Y (Captured), Is Asteroid Collected, Asteroid Collect Time, Asteroid's Coordinate X (Collected), Asteroid's Coordinate Y (Collected), Vacuum's Coordinate X (Captured), Vacuum's Coordinate Y (Captured)");

            // summary
            AppendLog(CSVType.Summary, $"Exercise #{_coreGameNum}");
            AppendLog(CSVType.Summary, $"Date, Time, Level, Captured Count, Collected Count, Total Count, Success Rate");

            // coordinates
            AppendLog(CSVType.Coordinates, $"Exercise #{_coreGameNum}");
            AppendLog(CSVType.Coordinates, $"Time, Cursor Coordinate X, Cursor Coordinate Y");
        }

        public void OnCoreGameAsteroidEnded(CSVCoreGameAsteroidEndedData data)
        {
            // coordinatePos
            AppendLog(CSVType.CoordinatePos, $"{data.asteroidId}, {data.asteroidSpawnTime}, {data.asteroidCoordinateWhenSpawned.x}, {data.asteroidCoordinateWhenSpawned.y}, {data.cursorCoordinateWhenAsteroidSpawned.x}, {data.cursorCoordinateWhenAsteroidSpawned.y}, {data.isAsteroidCaptured}, {data.timeSpentToCaptureAsteroid}, {data.asteroidCoordinateWhenCaptured.x}, {data.asteroidCoordinateWhenCaptured.y}, {data.cursorCoordinateWhenAsteroidCaptured.x}, {data.cursorCoordinateWhenAsteroidCaptured.y}, {data.isAsteroidCollected}, {data.timeSpentToCollectAsteroid}, {data.asteroidCoordinateWhenCollected.x}, {data.asteroidCoordinateWhenCollected.y}, {data.vacuumCenterCoordinateWhenCollected.x}, {data.vacuumCenterCoordinateWhenCollected.y}");
        }

        public void OnCoreGameTick(CSVCoreGameTickData data)
        {
            // coordinates
            AppendLog(CSVType.Coordinates, $"{data.coreGameTime}, {data.cursorCoordinate.x}, {data.cursorCoordinate.y}");
        }

        public void OnCoreGameEnded(CSVCoreGameEndedData data)
        {
            // summary
            AppendLog(CSVType.Summary, $"{_logTime.Month:00}/{_logTime.Day:00}/{_logTime.Year:0000}, {_logTime.Hour:00}/{_logTime.Minute:00}, {data.level}, {data.capturedCount}, {data.collectedCount}, {data.totalCount}, {data.successRate}%");

            TryEndLog();
        }
        #endregion

        #region Private
        private string SetupDayFolder(DateTime time)
        {
            var dayFolderPath = GetDayFolderPath(time, SettingManager.Instance.DisplayName);

            try
            {
                if (!Directory.Exists(dayFolderPath))
                {
                    Directory.CreateDirectory(dayFolderPath);
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }

            return dayFolderPath;
        }

        private string GetDayFolderName(DateTime time, string username)
        {
            return string.Format(DayFolderTemplate, time.Month, time.Day, time.Year, username);
        }

        private string GetDayFolderPath(DateTime time, string username)
        {
#if UNITY_EDITOR
            var dayFolderPath = Path.Combine(Application.dataPath, "CSVFiles", GetDayFolderName(time, username));
#else
            var dayFolderPath = Path.Combine(Application.persistentDataPath, "CSVFiles", GetDayFolderName(time, username));
#endif
            return dayFolderPath;
        }

        private string GetSessionFolderName(int sessionNum, DateTime time)
        {
            return string.Format(SessionFolderTemplate, sessionNum, time.Hour, time.Minute);
        }

        private string GetSessionFolderPath(string dayFolderPath, int sessionNum, DateTime time)
        {
            return Path.Combine(dayFolderPath, GetSessionFolderName(sessionNum, time));
        }

        private string GetCoordinatePosFileName(DateTime time, string username, int block, int level)
        {
            return string.Format(CoordinatePosFileTemplate, username, time.Month, time.Day, time.Year, block, level);
        }

        private string GetCoordinatePosFilePath(string sessionFolderPath, DateTime time, string username, int block, int level)
        {
            return Path.Combine(sessionFolderPath, $"{GetCoordinatePosFileName(time, username, block, level)}.csv");
        }

        private string GetCoordinatesFileName(DateTime time, string username, int block, int level)
        {
            return string.Format(CoordinatesFileTemplate, username, time.Month, time.Day, time.Year, block, level);
        }

        private string GetCoordinatesFilePath(string sessionFolderPath, DateTime time, string username, int block, int level)
        {
            return Path.Combine(sessionFolderPath, $"{GetCoordinatesFileName(time, username, block, level)}.csv");
        }

        private string GetSummaryFileName(string username)
        {
            return string.Format(SummaryFileTemplate, username);
        }

        private string GetSummaryFilePath(string sessionFolderPath, string username)
        {
            return Path.Combine(sessionFolderPath, $"{GetSummaryFileName(username)}.csv");
        }

        private bool TryParseSessionFolder(string sessionFolderName, out int sessionNum)
        {
            sessionNum = 1;

            var sessionStr = sessionFolderName.Substring(sessionFolderName.IndexOf(SessionNumPrefix) + SessionNumPrefix.Length);
            if (sessionStr.Length > 0)
            {
                var sessionNumStr = sessionStr.Substring(0, sessionStr.IndexOf('.'));
                if (int.TryParse(sessionNumStr, out sessionNum) && sessionNum >= 1)
                {
                    return true;
                }
            }

            Debug.LogError($"Invalid session folder name detected! Please check! -> {sessionFolderName}");
            return false;
        }

        /// <summary>
        /// Called when a logging session starts / ends
        /// </summary>
        /// <returns></returns>
        private bool TryEndLog()
        {
            if (_isLogging)
            {
                _isLogging = false;

                foreach (var pair in _logSbDict)
                {
                    var type = pair.Key;
                    var sb = pair.Value;
                    SaveLog(type, sb.ToString());
                }

                _calibrationNum = 0;
                _coreGameNum = 0;
                _sessionNum = 0;
                _logTime = DateTime.MinValue;
                _logSbDict.Clear();

                return true;
            }

            return false;
        }

        private void SaveLog(CSVType type, string content)
        {
            try
            {
                StreamWriter writer = null;
                switch (type)
                {
                    case CSVType.CoordinatePos:
                        if (!string.IsNullOrEmpty(_coordinatePosFilePath))
                        {
                            writer = new StreamWriter(_coordinatePosFilePath, false);
                        }
                        break;
                    case CSVType.Summary:
                        if (!string.IsNullOrEmpty(_summaryFilePath))
                        {
                            writer = new StreamWriter(_summaryFilePath, false);
                        }
                        break;
                    case CSVType.Coordinates:
                        if (!string.IsNullOrEmpty(_coordinatesFilePath))
                        {
                            writer = new StreamWriter(_coordinatesFilePath, false);
                        }
                        break;
                    default:
                        Logger.LogError($"Undefined CSVType: {type}");
                        break;
                }

                if (writer != null)
                {
                    writer.Write(content);
                    writer.WriteLine();
                    writer.Close();
                    writer = null;
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }

        /// <summary>
        /// Need to be called before writing any log to local file
        /// </summary>
        /// <param name="logTime">Generation time of the log</param>
        /// <returns>If success</returns>
        private bool StartLog(DateTime logTime)
        {
            TryEndLog();

            _isLogging = true;
            _logTime = logTime;

            try
            {
                var dayFolderPath = GetDayFolderPath(logTime, SettingManager.Instance.DisplayName);

                if (!Directory.Exists(dayFolderPath))
                {
                    Directory.CreateDirectory(dayFolderPath);
                    _sessionNum = 0;
                }

                // check existing session folders within day folder
                var sessionNum = 0;
                var sessionFolders = Directory.GetDirectories(dayFolderPath);
                foreach (var sessionFolder in sessionFolders)
                {
                    if (TryParseSessionFolder(sessionFolder, out var pSessionNum))
                    {
                        sessionNum = Mathf.Max(sessionNum, pSessionNum);
                    }
                }
                sessionNum = Mathf.Max(sessionNum, 0);

                if (_sessionNum == 0 || _sessionNum != sessionNum) // init session (folder and stuff)
                {
                    _sessionNum = sessionNum + 1;
                }

                var sessionFolderPath = GetSessionFolderPath(dayFolderPath, _sessionNum, logTime);
                if (!Directory.Exists(sessionFolderPath))
                {
                    Directory.CreateDirectory(sessionFolderPath);
                }

                _coordinatePosFilePath = GetCoordinatePosFilePath(sessionFolderPath, logTime, SettingManager.Instance.DisplayName, SettingManager.Instance.DailyBlock, SettingManager.Instance.Level);
                _coordinatesFilePath = GetCoordinatesFilePath(sessionFolderPath, logTime, SettingManager.Instance.DisplayName, SettingManager.Instance.DailyBlock, SettingManager.Instance.Level);
                _summaryFilePath = GetSummaryFilePath(sessionFolderPath, SettingManager.Instance.DisplayName);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return false;
            }

            Logger.Log($"CSVManager::Setup - _sessionNum: {_sessionNum} | _calibrationNum: {_calibrationNum} | _coreGameNum: {_coreGameNum} | _coordinatePosFilePath: {_coordinatePosFilePath} | _coordinatesFilePath: {_coordinatesFilePath} | _summaryFilePath: {_summaryFilePath}");
            return true;
        }

        private void AppendLog(CSVType type, string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (!_logSbDict.TryGetValue(type, out var sb))
            {
                sb = new StringBuilder();
                _logSbDict[type] = sb;
            }

            if (sb != null)
            {
                sb.Append(text);
            }
            else
            {
                Logger.LogError($"Cannot append log when _logSb is null!");
            }
        }
        #endregion
    }
}