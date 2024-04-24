using System.Collections;
using System.IO;
using UnityEngine;
using System.Text;
using System;
using System.Collections.Generic;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Globalization;

namespace com.hive.projectr
{
    public interface ICSVData
    {

    }

    #region CSV Data
    public struct CSVCoreGameEndedData : ICSVData
    {
        [Name("Level")]
        public int level;

        [Name("Captured Amount")]
        public int capturedCount;

        [Name("Collected Amount")]
        public int collectedCount;

        [Name("Total Amount")]
        public int totalCount;

        [Name("Success Rate")]
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

    public struct CSVCoreGameTickData : ICSVData
    {
        [Name("Time")]
        public float coreGameTime;

        [Name("Cursor Coordinate")]
        public Vector2 cursorCoordinate;

        public CSVCoreGameTickData(float coreGameTime, Vector2 cursorCoordinate)
        {
            this.coreGameTime = coreGameTime;
            this.cursorCoordinate = cursorCoordinate;
        }
    }

    public struct CSVCoreGameAsteroidEndedData : ICSVData
    {
        [Name("Asteroid ID")]
        public int asteroidId;

        // spawn
        [Name("Asteroid Spawn Time")]
        public float asteroidSpawnTime;

        [Name("Asteroid Coordinate When Spawned")]
        public Vector2 asteroidCoordinateWhenSpawned;

        [Name("Cursor Coordinate When Asteroid Spawned")]
        public Vector2 cursorCoordinateWhenAsteroidSpawned;

        // capture
        [Name("Is Asteroid Captured")]
        public bool isAsteroidCaptured;
        
        [Name("Time To Capture")]
        public float timeSpentToCaptureAsteroid;

        [Name("Asteroid Coordinate When Captured")]
        public Vector2 asteroidCoordinateWhenCaptured;

        [Name("Cursor Coordinate When Asteroid Captured")]
        public Vector2 cursorCoordinateWhenAsteroidCaptured;

        // collect
        [Name("Is Asteroid Collected")]
        public bool isAsteroidCollected;

        [Name("Time To Collect")]
        public float timeSpentToCollectAsteroid;
        
        [Name("Asteroid Coordinate When Collected")]
        public Vector2 asteroidCoordinateWhenCollected;

        [Name("Vacuum Center Coordinate When Asteroid Collected")]
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

    public struct CSVCalibrationEndedData : ICSVData
    {
        [Name("Center Coordinate")]
        public Vector2 center;

        [Name("Top Left Coordinate")]
        public Vector2 topLeft;

        [Name("Top Right Coordinate")]
        public Vector2 topRight;

        [Name("Bottom Right Coordinate")]
        public Vector2 bottomRight;

        [Name("Bottom Left Coordinate")]
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

    public struct CSVSessionInfo : ICSVData
    {
        public int calibrationNum;
        public int coreGameNum;

        public CSVSessionInfo(int calibrationNum, int coreGameNum)
        {
            this.calibrationNum = calibrationNum;
            this.coreGameNum = coreGameNum;
        }
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
        private string _sessionInfoFilePath;

        private static readonly string DayFolderTemplate = "{0:00}-{1:00}-{2:0000}_{3}"; // 03-13-2024_Mark
        private static readonly string SessionFolderTemplate = "Session#{0}_{1:00}-{2:00}"; // Session#3_14-25
        private static readonly string SessionNumPrefix = "Session#";
        private static readonly string CoordinatePosFileTemplate = "{0}_{1}-{2}-{3}_{4}Block_Level{5}_CoordinatePOS"; // Mark_03-13-2024_100Block_Level1_CoordinatePOS
        private static readonly string CoordinatesFileTemplate = "{0}_{1}-{2}-{3}_{4}Block_Level{5}_Coordinates"; // Mark_03-13-2024_100Block_Level1_Coordinates
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
            Logger.Log($"CSVManager::OnCalibrationStarted - logTime: {logTime}");

            StartLog(logTime);

            ++_calibrationNum;

            // coordinatePos
            AppendLog(CSVType.CoordinatePos, $"Calibration #{_calibrationNum}\n");
            AppendLog(CSVType.CoordinatePos, GenerateCSVHeader<CSVCalibrationEndedData>());
        }

        /// <summary>
        /// Called when a calibration is finished.
        /// Save calibration CSV data.
        /// </summary>
        /// <param name="data"></param>
        public void OnCalibrationEnded(CSVCalibrationEndedData data)
        {
            Logger.Log($"CSVManager::OnCalibrationEnded");

            // coordinatePos
            AppendLog(CSVType.CoordinatePos, GenerateCSVContent(new List<CSVCalibrationEndedData>() { data }));

            TryEndLog();
        }
        #endregion

        #region Core Game
        public void OnCoreGameStarted(DateTime logTime)
        {
            Logger.Log($"CSVManager::OnCoreGameStarted - logTime: {logTime}");

            StartLog(logTime);

            ++_coreGameNum;

            // coordinatePos
            AppendLog(CSVType.CoordinatePos, $"Exercise #{_coreGameNum}\n");
            AppendLog(CSVType.CoordinatePos, GenerateCSVHeader<CSVCoreGameAsteroidEndedData>());

            // summary
            AppendLog(CSVType.Summary, $"Exercise #{_coreGameNum}\n");
            AppendLog(CSVType.Summary, GenerateCSVHeader<CSVCoreGameEndedData>());

            // coordinates
            AppendLog(CSVType.Coordinates, $"Exercise #{_coreGameNum}\n");
            AppendLog(CSVType.Coordinates, GenerateCSVHeader<CSVCoreGameTickData>());
        }

        public void OnCoreGameAsteroidEnded(CSVCoreGameAsteroidEndedData data)
        {
            Logger.Log($"CSVManager::OnCoreGameAsteroidEnded");

            // coordinatePos
            AppendLog(CSVType.Coordinates, GenerateCSVContent(new List<CSVCoreGameAsteroidEndedData>() { data }));
        }

        public void OnCoreGameTick(CSVCoreGameTickData data)
        {
            Logger.Log($"CSVManager::OnCoreGameTick");

            // coordinates
            AppendLog(CSVType.Coordinates, GenerateCSVContent(new List<CSVCoreGameTickData>() { data }));
        }

        public void OnCoreGameEnded(CSVCoreGameEndedData data)
        {
            Logger.Log($"CSVManager::OnCoreGameEnded");

            // summary
            AppendLog(CSVType.Summary, GenerateCSVContent(new List<CSVCoreGameEndedData>() { data }));

            TryEndLog();
        }
        #endregion

        #region Private
        private string SetupDayFolder(DateTime time)
        {
            var dayFolderPath = GetDayFolderPath(time, SettingManager.Instance.DisplayName);

            try
            {
                Directory.CreateDirectory(dayFolderPath); // does nothing when already exists
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }

            Logger.Log($"CSVManager::SetupDayFolder - dayFolderPath: {dayFolderPath}");

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

        private string GetSessionInfoFilePath(string sessionFolderPath)
        {
            return Path.Combine(sessionFolderPath, $"Info.txt");
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
            try
            {
                if (_isLogging)
                {
                    _isLogging = false;

                    // save
                    foreach (var pair in _logSbDict)
                    {
                        var type = pair.Key;
                        var sb = pair.Value;
                        SaveLog(type, sb.ToString());
                    }

                    if (File.Exists(_sessionInfoFilePath))
                    {
                        using (var writer = new StreamWriter(_sessionInfoFilePath, false))
                        {
                            writer.WriteLine(_calibrationNum);
                            writer.WriteLine(_coreGameNum);
                        }
                    }

                    _calibrationNum = 0;
                    _coreGameNum = 0;
                    _sessionNum = 0;
                    _coordinatePosFilePath = null;
                    _coordinatesFilePath = null;
                    _summaryFilePath = null;
                    _sessionInfoFilePath = null;
                    _logTime = DateTime.MinValue;
                    _logSbDict.Clear();

                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }

            return false;
        }

        private void SaveLog(CSVType type, string content)
        {
            Logger.Log($"CSVManager::SaveLog - type: {type} | content: {content}");

            try
            {
                switch (type)
                {
                    case CSVType.CoordinatePos:
                        if (!string.IsNullOrEmpty(_coordinatePosFilePath))
                        {
                            using (var writer = new StreamWriter(_coordinatePosFilePath, false))
                            {
                                if (writer != null)
                                {
                                    writer.Write(content);
                                    writer.WriteLine();
                                    writer.Close();
                                }
                            }
                        }
                        break;
                    case CSVType.Summary:
                        if (!string.IsNullOrEmpty(_summaryFilePath))
                        {
                            using (var writer = new StreamWriter(_summaryFilePath, false))
                            {
                                if (writer != null)
                                {
                                    writer.Write(content);
                                    writer.WriteLine();
                                    writer.Close();
                                }
                            }
                        }
                        break;
                    case CSVType.Coordinates:
                        if (!string.IsNullOrEmpty(_coordinatesFilePath))
                        {
                            using (var writer = new StreamWriter(_coordinatesFilePath, false))
                            {
                                if (writer != null)
                                {
                                    writer.Write(content);
                                    writer.WriteLine();
                                    writer.Close();
                                }
                            }
                        }
                        break;
                    default:
                        Logger.LogError($"Undefined CSVType: {type}");
                        break;
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
                Directory.CreateDirectory(sessionFolderPath); // does nothing when already exists

                _coordinatePosFilePath = GetCoordinatePosFilePath(sessionFolderPath, logTime, SettingManager.Instance.DisplayName, SettingManager.Instance.DailyBlock, SettingManager.Instance.Level);
                _coordinatesFilePath = GetCoordinatesFilePath(sessionFolderPath, logTime, SettingManager.Instance.DisplayName, SettingManager.Instance.DailyBlock, SettingManager.Instance.Level);
                _summaryFilePath = GetSummaryFilePath(sessionFolderPath, SettingManager.Instance.DisplayName);
                
                // read session info
                _sessionInfoFilePath = GetSessionInfoFilePath(sessionFolderPath);
                if (File.Exists(_sessionInfoFilePath))
                {
                    using (var reader = new StreamReader(_sessionInfoFilePath))
                    {
                        if (!int.TryParse(reader.ReadLine(), out _calibrationNum))
                        {
                            Logger.LogError($"Failed to parse calibration num from: {_sessionInfoFilePath}");
                        }

                        if (!int.TryParse(reader.ReadLine(), out _coreGameNum))
                        {
                            Logger.LogError($"Failed to parse core game num from: {_sessionInfoFilePath}");
                        }
                    }
                }
                else
                {
                    _coreGameNum = 0;
                    _calibrationNum = 0;
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                Logger.Log($"CSVManager::Setup - _sessionNum: {_sessionNum} | _calibrationNum: {_calibrationNum} | _coreGameNum: {_coreGameNum} | _coordinatePosFilePath: {_coordinatePosFilePath} | _coordinatesFilePath: {_coordinatesFilePath} | _summaryFilePath: {_summaryFilePath}");
                return false;
            }

            Logger.Log($"CSVManager::Setup - _sessionNum: {_sessionNum} | _calibrationNum: {_calibrationNum} | _coreGameNum: {_coreGameNum} | _coordinatePosFilePath: {_coordinatePosFilePath} | _coordinatesFilePath: {_coordinatesFilePath} | _summaryFilePath: {_summaryFilePath}");
            return true;
        }

        public string GenerateCSVHeader<T>() where T : ICSVData
        {
            using (var writer = new StringWriter())
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader(typeof(T));
                return writer.ToString();
            }
        }

        public string GenerateCSVContent<T>(IEnumerable<T> records) where T : ICSVData
        {
            using (var writer = new StringWriter())
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
                return writer.ToString();
            }
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