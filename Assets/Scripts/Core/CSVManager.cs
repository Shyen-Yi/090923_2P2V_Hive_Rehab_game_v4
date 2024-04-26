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
        public int Level { get; private set; }

        [Name("Captured Amount")]
        public int CapturedCount { get; private set; }

        [Name("Collected Amount")]
        public int CollectedCount { get; private set; }

        [Name("Total Amount")]
        public int TotalCount { get; private set; }

        [Name("Success Rate")]
        public int SuccessRate { get; private set; } // percentage

        public CSVCoreGameEndedData(int level, int capturedCount, int collectedCount, int totalCount, int successRate)
        {
            this.Level = level;
            this.CapturedCount = capturedCount;
            this.CollectedCount = collectedCount;
            this.TotalCount = totalCount;
            this.SuccessRate = successRate;
        }
    }

    public struct CSVCoreGameTickData : ICSVData
    {
        [Name("Time")]
        public float CoreGameTime { get; private set; }

        [Name("Cursor Coordinate X")]
        public float CursorCoordinateX { get; private set; }

        [Name("Cursor Coordinate Y")]
        public float CursorCoordinateY { get; private set; }

        public CSVCoreGameTickData(float coreGameTime, Vector2 cursorCoordinate)
        {
            CoreGameTime = coreGameTime;
            CursorCoordinateX = cursorCoordinate.x;
            CursorCoordinateY = cursorCoordinate.y;
        }
    }

    public struct CSVCoreGameAsteroidEndedData : ICSVData
    {
        [Name("Asteroid ID")]
        public int AsteroidId { get; private set; }

        // spawn
        [Name("Asteroid Spawn Time")]
        public float AsteroidSpawnTime { get; private set; }

        [Name("Asteroid Coordinate When Spawned X")]
        public float AsteroidCoordinateWhenSpawnedX { get; private set; }

        [Name("Asteroid Coordinate When Spawned Y")]
        public float AsteroidCoordinateWhenSpawnedY { get; private set; }

        [Name("Cursor Coordinate When Asteroid Spawned X")]
        public float CursorCoordinateWhenAsteroidSpawnedX { get; private set; }

        [Name("Cursor Coordinate When Asteroid Spawned Y")]
        public float CursorCoordinateWhenAsteroidSpawnedY { get; private set; }

        // capture
        [Name("Is Asteroid Captured")]
        public bool IsAsteroidCaptured { get; private set; }
        
        [Name("Time To Capture")]
        public float TimeSpentToCaptureAsteroid { get; private set; }

        [Name("Asteroid Coordinate When Captured X")]
        public float AsteroidCoordinateWhenCapturedX { get; private set; }

        [Name("Asteroid Coordinate When Captured Y")]
        public float AsteroidCoordinateWhenCapturedY { get; private set; }

        [Name("Cursor Coordinate When Asteroid Captured X")]
        public float CursorCoordinateWhenAsteroidCapturedX { get; private set; }

        [Name("Cursor Coordinate When Asteroid Captured Y")]
        public float CursorCoordinateWhenAsteroidCapturedY { get; private set; }

        // collect
        [Name("Is Asteroid Collected")]
        public bool IsAsteroidCollected { get; private set; }

        [Name("Time To Collect")]
        public float TimeSpentToCollectAsteroid { get; private set; }
        
        [Name("Asteroid Coordinate When Collected X")]
        public float AsteroidCoordinateWhenCollectedX { get; private set; }

        [Name("Asteroid Coordinate When Collected Y")]
        public float AsteroidCoordinateWhenCollectedY { get; private set; }

        [Name("Vacuum Center Coordinate When Asteroid Collected X")]
        public float VacuumCenterCoordinateWhenCollectedX { get; private set; }

        [Name("Vacuum Center Coordinate When Asteroid Collected Y")]
        public float VacuumCenterCoordinateWhenCollectedY { get; private set; }

        public CSVCoreGameAsteroidEndedData(int asteroidId, float asteroidSpawnTime, Vector2 asteroidCoordinateWhenSpawned, Vector2 cursorCoordinateWhenAsteroidSpawned, bool isAsteroidCaptured, float timeSpentToCaptureAsteroid, Vector2 asteroidCoordinateWhenCaptured, Vector2 cursorCoordinateWhenAsteroidCaptured, bool isAsteroidCollected, float timeSpentToCollectAsteroid, Vector2 asteroidCoordinateWhenCollected, Vector2 vacuumCenterCoordinateWhenCollected)
        {
            AsteroidId = asteroidId;
            AsteroidSpawnTime = asteroidSpawnTime;
            AsteroidCoordinateWhenSpawnedX = asteroidCoordinateWhenSpawned.x;
            AsteroidCoordinateWhenSpawnedY = asteroidCoordinateWhenSpawned.y;
            CursorCoordinateWhenAsteroidSpawnedX = cursorCoordinateWhenAsteroidSpawned.x;
            CursorCoordinateWhenAsteroidSpawnedY = cursorCoordinateWhenAsteroidSpawned.y;
            IsAsteroidCaptured = isAsteroidCaptured;
            TimeSpentToCaptureAsteroid = timeSpentToCaptureAsteroid;
            AsteroidCoordinateWhenCapturedX = asteroidCoordinateWhenCaptured.x;
            AsteroidCoordinateWhenCapturedY = asteroidCoordinateWhenCaptured.y;
            CursorCoordinateWhenAsteroidCapturedX = cursorCoordinateWhenAsteroidCaptured.x;
            CursorCoordinateWhenAsteroidCapturedY = cursorCoordinateWhenAsteroidCaptured.y;
            IsAsteroidCollected = isAsteroidCollected;
            TimeSpentToCollectAsteroid = timeSpentToCollectAsteroid;
            AsteroidCoordinateWhenCollectedX = asteroidCoordinateWhenCollected.x;
            AsteroidCoordinateWhenCollectedY = asteroidCoordinateWhenCollected.y;
            VacuumCenterCoordinateWhenCollectedX = vacuumCenterCoordinateWhenCollected.x;
            VacuumCenterCoordinateWhenCollectedY = vacuumCenterCoordinateWhenCollected.y;
        }
    }

    public struct CSVCalibrationEndedData : ICSVData
    {
        [Name("Center Coordinate X")]
        public float CenterX { get; private set; }

        [Name("Center Coordinate X")]
        public float CenterY { get; private set; }

        [Name("Top Left Coordinate X")]
        public float TopLeftX { get; private set; }

        [Name("Top Left Coordinate Y")]
        public float TopLeftY { get; private set; }

        [Name("Top Right Coordinate X")]
        public float TopRightX { get; private set; }

        [Name("Top Right Coordinate Y")]
        public float TopRightY { get; private set; }

        [Name("Bottom Right Coordinate X")]
        public float BottomRightX { get; private set; }

        [Name("Bottom Right Coordinate Y")]
        public float BottomRightY { get; private set; }

        [Name("Bottom Left Coordinate X")]
        public float BottomLeftX { get; private set; }

        [Name("Bottom Left Coordinate Y")]
        public float BottomLeftY { get; private set; }

        public CSVCalibrationEndedData(Vector2 center, Vector2 topLeft, Vector2 topRight, Vector2 bottomRight, Vector2 bottomLeft)
        {
            CenterX = center.x;
            CenterY = center.y;
            TopLeftX = topLeft.x;
            TopLeftY = topLeft.y;
            TopRightX = topRight.x;
            TopRightY = topRight.y;
            BottomRightX = bottomRight.x;
            BottomRightY = bottomRight.y;
            BottomLeftX = bottomLeft.x;
            BottomLeftY = bottomLeft.y;
        }
    }

    public struct CSVSessionInfo : ICSVData
    {
        public int CalibrationNum { get; set; }
        public int CoreGameNum { get; set; }

        public CSVSessionInfo(int calibrationNum, int coreGameNum)
        {
            CalibrationNum = calibrationNum;
            CoreGameNum = coreGameNum;
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
        private Dictionary<CSVType, StringBuilder> _logSbDict = new Dictionary<CSVType, StringBuilder>();

        private string _coordinatePosFilePath;
        private string _coordinatesFilePath;
        private string _summaryFilePath;
        private string _sessionFolderPath;
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
            AppendLog(CSVType.CoordinatePos, GenerateCSVContent(new List<CSVCoreGameAsteroidEndedData>() { data }));
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
            return Path.Combine(sessionFolderPath, $"SessionInfo.txt");
        }

        private bool TryParseSessionFolder(string sessionFolderName, out int sessionNum)
        {
            sessionNum = 1;

            var sessionStr = sessionFolderName.Substring(sessionFolderName.IndexOf(SessionNumPrefix) + SessionNumPrefix.Length);
            if (sessionStr.Length > 0)
            {
                var sessionNumStr = sessionStr.Substring(0, sessionStr.IndexOf('_'));
                if (int.TryParse(sessionNumStr, out sessionNum) && sessionNum >= 1)
                {
                    return true;
                }
            }

            Logger.LogError($"Invalid session folder name detected! Please check! -> {sessionFolderName}");
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
                        if (sb != null)
                        {
                            var str = sb.ToString();
                            if (str.Length > 0)
                            {
                                SaveLog(type, str);
                            }
                        }
                    }

                    SaveSessionInfo(_sessionInfoFilePath, new CSVSessionInfo(_calibrationNum, _coreGameNum));

                    _coordinatePosFilePath = null;
                    _coordinatesFilePath = null;
                    _summaryFilePath = null;
                    _sessionInfoFilePath = null;
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

        private void SaveSessionInfo(string path, CSVSessionInfo info)
        {
            try
            {
                using (var writer = new StreamWriter(path, false))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteHeader<CSVSessionInfo>();
                    csv.NextRecord();
                    csv.WriteRecord(info);
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
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
                            using (var writer = new StreamWriter(_coordinatePosFilePath, true))
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
                            using (var writer = new StreamWriter(_summaryFilePath, true))
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
                            using (var writer = new StreamWriter(_coordinatesFilePath, true))
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
            var isSuccess = true;

            TryEndLog();

            _isLogging = true;

            try
            {
                var dayFolderPath = GetDayFolderPath(logTime, SettingManager.Instance.DisplayName);
                var isNewSession = false;

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
                    isNewSession = true;
                }

                if (isNewSession)
                {
                    _sessionFolderPath = GetSessionFolderPath(dayFolderPath, _sessionNum, logTime);
                }

                Directory.CreateDirectory(_sessionFolderPath); // does nothing when already exists

                _coordinatePosFilePath = GetCoordinatePosFilePath(_sessionFolderPath, logTime, SettingManager.Instance.DisplayName, SettingManager.Instance.DailyBlock, SettingManager.Instance.Level);
                _coordinatesFilePath = GetCoordinatesFilePath(_sessionFolderPath, logTime, SettingManager.Instance.DisplayName, SettingManager.Instance.DailyBlock, SettingManager.Instance.Level);
                _summaryFilePath = GetSummaryFilePath(_sessionFolderPath, SettingManager.Instance.DisplayName);
                
                // read session info
                _sessionInfoFilePath = GetSessionInfoFilePath(_sessionFolderPath);

                if (File.Exists(_sessionInfoFilePath))
                {
                    using (var reader = new StreamReader(_sessionInfoFilePath))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        csv.Read();
                        csv.ReadHeader();

                        var infos = csv.GetRecords<CSVSessionInfo>();
                        foreach (var info in infos)
                        {
                            _calibrationNum = info.CalibrationNum;
                            _coreGameNum = info.CoreGameNum;
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
                isSuccess = false;
            }

            Logger.Log($"CSVManager::StartLog - _sessionNum: {_sessionNum} | _sessionFolderPath: {_sessionFolderPath} | _sessionInfoFilePath: {_sessionInfoFilePath} | _calibrationNum: {_calibrationNum} | _coreGameNum: {_coreGameNum} | _coordinatePosFilePath: {_coordinatePosFilePath} | _coordinatesFilePath: {_coordinatesFilePath} | _summaryFilePath: {_summaryFilePath}");
            return isSuccess;
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