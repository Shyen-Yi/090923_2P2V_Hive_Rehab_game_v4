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
    /// <summary>
    /// Implemented by all CSVData. A placeholder for structure and scalability.
    /// </summary>
    public interface ICSVData
    {

    }

    #region CSV Data
    /// <summary>
    /// Data retrieved when a core game session is completed.
    /// </summary>
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

    /// <summary>
    /// Data retrieved at every "tick" of a core game session.
    /// </summary>
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

    /// <summary>
    /// Data retrieved when an asteroid's lifecycle comes to an end (captured/self-destroyed).
    /// </summary>
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

    /// <summary>
    /// Data retrieved when a calibration session is completed.
    /// </summary>
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

    /// <summary>
    /// Stores #Calibration and #CoreGame for an entire game session.
    /// </summary>
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

    /// <summary>
    /// Different types of output CSV logs.
    /// </summary>
    public enum CSVType
    {
        CoordinatePos,
        Summary,
        Coordinates,
    }

    /// @ingroup Core
    /// @class CSVManager
    /// @brief Manages CSV logs for core game events such as game progress, coordinates, and summaries.
    ///
    /// This class handles the logging of game events to CSV files, including the creation of day folders
    /// for organizing logs and methods for appending data to different CSV types (e.g., Coordinates, Summary).
    public class CSVManager : SingletonBase<CSVManager>, ICoreManager
    {
        /// <summary>
        /// The number of daily attempts made by the player.
        /// </summary>
        public int DailyAttempt
        {
            get => _dailyAttempt;
            private set
            {
                _dailyAttempt = value;
            }
        }
        private int _dailyAttempt;

        private bool _isLogging;
        private bool _isCoreGame;

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
        private static readonly string CoordinatePosFileTemplate = "{0}_{1}-{2}-{3}_{4}Block_{5}Goal_Level{6}_CoordinatePOS"; // Mark_03-13-2024_100Block_80Goal_Level1_CoordinatePOS
        private static readonly string CoordinatesFileTemplate = "{0}_{1}-{2}-{3}_{4}Block_{5}Goal_Level{6}_Coordinates"; // Mark_03-13-2024_100Block_80Goal_Level1_Coordinates
        private static readonly string SummaryFileTemplate = "{0}_Summary"; // Mark_Summary

        #region Lifecycle
        /// <summary>
        /// Initializes the CSVManager instance.
        /// This method is called when the CSVManager is initialized and subscribes to relevant events.
        /// </summary>
        public void OnInit()
        {
            TrySetupDayFolder(TimeUtil.Now, out var dayFolderPath);
            InitDailyAttempt();

            MonoBehaviourUtil.OnApplicationQuitEvent += OnApplicationQuit;
            SettingManager.OnDisplayNameUpdated += OnDisplayNameChanged;
            TimeManager.OnGameDayUpdated += OnGameDayUpdated;
        }

        /// <summary>
        /// Disposes of the CSVManager instance.
        /// This method is called when the CSVManager is disposed and unsubscribes from events.
        /// </summary>
        public void OnDispose()
        {
            MonoBehaviourUtil.OnApplicationQuitEvent -= OnApplicationQuit;
            SettingManager.OnDisplayNameUpdated -= OnDisplayNameChanged;
            TimeManager.OnGameDayUpdated -= OnGameDayUpdated;
        }
        #endregion

        #region Event
        /// <summary>
        /// Triggered when the application quits.
        /// Ends any active logging sessions.
        /// </summary>
        private void OnApplicationQuit()
        {
            TryEndLog();
        }

        /// <summary>
        /// Triggered when the display name is updated.
        /// Reinitializes the daily attempt count.
        /// </summary>
        private void OnDisplayNameChanged()
        {
            InitDailyAttempt();
        }

        /// <summary>
        /// Triggered when the game day is updated.
        /// Reinitializes the daily attempt count.
        /// </summary>
        private void OnGameDayUpdated()
        {
            InitDailyAttempt();
        }
        #endregion

        #region Calibration
        /// <summary>
        /// Called when a calibration session is started.
        /// Begins logging and initializes data for the calibration.
        /// </summary>
        /// <param name="logTime">The timestamp for when the calibration starts.</param>
        public void OnCalibrationStarted(DateTime logTime)
        {
            Logger.Log($"CSVManager::OnCalibrationStarted - logTime: {logTime}");

            StartLog(logTime, false);

            ++_calibrationNum;

            // coordinatePos
            AppendLog(CSVType.CoordinatePos, $"Calibration #{_calibrationNum}\n");
            AppendLog(CSVType.CoordinatePos, GenerateCSVHeader<CSVCalibrationEndedData>());
        }

        /// <summary>
        /// Called when a calibration session ends.
        /// Appends the calibration data to the log.
        /// </summary>
        /// <param name="data">The calibration data to be logged.</param>
        public void OnCalibrationEnded(CSVCalibrationEndedData data)
        {
            Logger.Log($"CSVManager::OnCalibrationEnded");

            // coordinatePos
            AppendLog(CSVType.CoordinatePos, GenerateCSVContent(new List<CSVCalibrationEndedData>() { data }));

            TryEndLog();
        }
        #endregion

        #region Core Game
        /// <summary>
        /// Called when a core game session starts.
        /// Initializes logging and appends header data for the core game session.
        /// </summary>
        /// <param name="logTime">The timestamp for when the core game starts.</param>
        public void OnCoreGameStarted(DateTime logTime)
        {
            Logger.Log($"CSVManager::OnCoreGameStarted - logTime: {logTime}");

            StartLog(logTime, true);

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

        /// <summary>
        /// Called when an asteroid in the core game ends (either captured or self-destroyed).
        /// Appends asteroid-related data to the log.
        /// </summary>
        /// <param name="data">The asteroid data to be logged.</param>
        public void OnCoreGameAsteroidEnded(CSVCoreGameAsteroidEndedData data)
        {
            Logger.Log($"CSVManager::OnCoreGameAsteroidEnded");

            // coordinatePos
            AppendLog(CSVType.CoordinatePos, GenerateCSVContent(new List<CSVCoreGameAsteroidEndedData>() { data }));
        }

        /// <summary>
        /// Called every tick in the core game.
        /// Appends the current tick data to the log.
        /// </summary>
        /// <param name="data">The core game tick data to be logged.</param>
        public void OnCoreGameTick(CSVCoreGameTickData data)
        {
            //Logger.Log($"CSVManager::OnCoreGameTick");

            // coordinates
            AppendLog(CSVType.Coordinates, GenerateCSVContent(new List<CSVCoreGameTickData>() { data }));
        }

        /// <summary>
        /// Called when the core game ends.
        /// Appends the summary data to the log.
        /// </summary>
        /// <param name="data">The summary data to be logged.</param>
        public void OnCoreGameEnded(CSVCoreGameEndedData data)
        {
            Logger.Log($"CSVManager::OnCoreGameEnded");

            // summary
            AppendLog(CSVType.Summary, GenerateCSVContent(new List<CSVCoreGameEndedData>() { data }));

            TryEndLog();
        }
        #endregion

        #region Private
        /// <summary>
        /// Sets up the day folder for storing logs based on the current date.
        /// </summary>
        private bool TrySetupDayFolder(DateTime time, out string dayFolderPath)
        {
            dayFolderPath = "";

            if (SettingManager.Instance.IsDefaultUser)
            {
                return false;
            }

            dayFolderPath = GetDayFolderPath(time, SettingManager.Instance.DisplayName);

            Logger.Log($"CSVManager::SetupDayFolder - dayFolderPath: {dayFolderPath}");

            try
            {
                Directory.CreateDirectory(dayFolderPath); // does nothing when already exists
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the root storage path for all log files.
        /// </summary>
        /// <returns>The storage path for all log files.</returns>
        private string GetRootPath()
        {
#if UNITY_EDITOR
            var rootPath = Path.Combine(Application.dataPath, "CSVFiles");
#else
            var rootPath = Path.Combine(Application.persistentDataPath, "CSVFiles");
#endif
            return rootPath;
        }

        /// <summary>
        /// Gets the folder name for the log files generated by a specific player at a specific time.
        /// </summary>
        /// <param name="time">Current time as a DateTime object.</param>
        /// <param name="username">Current player's username.</param>
        /// <returns>The folder name for the log files generated by the given player at the given time.</returns>
        private string GetDayFolderName(DateTime time, string username)
        {
            return string.Format(DayFolderTemplate, time.Month, time.Day, time.Year, username);
        }

        /// <summary>
        /// Gets the storage path for all log files generated by a specific player at a specific time.
        /// </summary>
        /// <param name="time">Current time as a DateTime object.</param>
        /// <param name="username">Current player's username.</param>
        /// <returns>The storage path for all log files generated by the given player at the given time.</returns>
        private string GetDayFolderPath(DateTime time, string username)
        {
            var dayFolderPath = Path.Combine(GetRootPath(), GetDayFolderName(time, username));
            return dayFolderPath;
        }

        /// <summary>
        /// Gets the folder name for the log files generated within a specific session at a specific time.
        /// </summary>
        /// <param name="sessionNum">Current session number as an integer.</param>
        /// <param name="time">Current time as a DateTime object.</param>
        /// <returns>The folder name for the log files generated within the given session at the given time.</returns>
        private string GetSessionFolderName(int sessionNum, DateTime time)
        {
            return string.Format(SessionFolderTemplate, sessionNum, time.Hour, time.Minute);
        }

        /// <summary>
        /// Gets the storage path for the log files generated within a specific session at a specific time with a day folder.
        /// </summary>
        /// <param name="dayFolderPath">The day folder path for the current time.</param>
        /// <param name="sessionNum">Current session number.</param>
        /// <param name="time">Current time as a DateTime object.</param>
        /// <returns></returns>
        private string GetSessionFolderPath(string dayFolderPath, int sessionNum, DateTime time)
        {
            return Path.Combine(dayFolderPath, GetSessionFolderName(sessionNum, time));
        }

        /// <summary>
        /// Gets the name of the Coordinate Position log file.
        /// </summary>
        /// <param name="time">Current time as a DateTime object.</param>
        /// <param name="username">Current player's username.</param>
        /// <param name="block">Current level's block.</param>
        /// <param name="goal">Current level's goal.</param>
        /// <param name="level">Current level.</param>
        /// <returns>Name of the Coordinate Position log file.</returns>
        private string GetCoordinatePosFileName(DateTime time, string username, int block, int goal, int level)
        {
            return string.Format(CoordinatePosFileTemplate, username, time.Month, time.Day, time.Year, block, goal, level);
        }

        /// <summary>
        /// Gets the storage path of the Coordinate Position log file.
        /// </summary>
        /// <param name="sessionFolderPath">Current session folder path.</param>
        /// <param name="time">Current time.</param>
        /// <param name="username">Current player's username.</param>
        /// <param name="block">Current level's block.</param>
        /// <param name="goal">Current level's goal.</param>
        /// <param name="level">Current level.</param>
        /// <returns>Storage path of the Coordinate Position log file.</returns>
        private string GetCoordinatePosFilePath(string sessionFolderPath, DateTime time, string username, int block, int goal, int level)
        {
            return Path.Combine(sessionFolderPath, $"{GetCoordinatePosFileName(time, username, block, goal, level)}.csv");
        }

        /// <summary>
        /// Gets the name of the Coordinates log file.
        /// </summary>
        /// <param name="time">Current time as a DateTime object.</param>
        /// <param name="username">Current player's username.</param>
        /// <param name="block">Current level's block.</param>
        /// <param name="goal">Current level's goal.</param>
        /// <param name="level">Current level.</param>
        /// <returns>Name of the Coordinates log file.</returns>
        private string GetCoordinatesFileName(DateTime time, string username, int block, int goal, int level)
        {
            return string.Format(CoordinatesFileTemplate, username, time.Month, time.Day, time.Year, block, goal, level);
        }

        /// <summary>
        /// Gets the storage path of the Coordinates log file.
        /// </summary>
        /// <param name="sessionFolderPath">Current session folder path.</param>
        /// <param name="time">Current time.</param>
        /// <param name="username">Current player's username.</param>
        /// <param name="block">Current level's block.</param>
        /// <param name="goal">Current level's goal.</param>
        /// <param name="level">Current level.</param>
        /// <returns>Storage path of the Coordinates log file.</returns>
        private string GetCoordinatesFilePath(string sessionFolderPath, DateTime time, string username, int block, int goal, int level)
        {
            return Path.Combine(sessionFolderPath, $"{GetCoordinatesFileName(time, username, block, goal, level)}.csv");
        }

        /// <summary>
        /// Gets the name of the Summary log file.
        /// </summary>
        /// <param name="username">Current player's username.</param>
        /// <returns>Name of the Summary log file.</returns>
        private string GetSummaryFileName(string username)
        {
            return string.Format(SummaryFileTemplate, username);
        }

        /// <summary>
        /// Gets the storage path of the Summary log file.
        /// </summary>
        /// <param name="sessionFolderPath">Current session folder path.</param>
        /// <param name="username">Current player's username.</param>
        /// <returns>Storage path of the Summary log file.</returns>
        private string GetSummaryFilePath(string sessionFolderPath, string username)
        {
            return Path.Combine(sessionFolderPath, $"{GetSummaryFileName(username)}.csv");
        }

        /// <summary>
        /// Gets the storage path of the Session Info file path.
        /// </summary>
        /// <param name="sessionFolderPath">Current session folder path.</param>
        /// <returns>Storage path of the Session Info file path.</returns>
        private string GetSessionInfoFilePath(string sessionFolderPath)
        {
            return Path.Combine(sessionFolderPath, $"SessionInfo.txt");
        }

        /// <summary>
        /// Attempts to retrieve a session number from a session folder name.
        /// </summary>
        /// <param name="sessionFolderName">A session folder name.</param>
        /// <param name="sessionNum">Retrieved session number.</param>
        /// <returns>True if parsing is successfully; otherwise, false.</returns>
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
        /// Ends the current logging session and saves the logs to files.
        /// </summary>
        private bool TryEndLog()
        {
            Logger.Log($"CSVManager::TryEndLog - _isLogging: {_isLogging} | _isCoreGame: {_isCoreGame} | _logSbDict: {string.Join(",", _logSbDict)}");

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

                    if (_isCoreGame)
                    {
                        ++DailyAttempt;
                    }

                    _isCoreGame = false;

                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }

            return false;
        }

        /// <summary>
        /// Saves the 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="info"></param>
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

        /// <summary>
        /// Saves the log content to a specified log file.
        /// </summary>
        /// <param name="type">The type of log (e.g., Coordinates, Summary).</param>
        /// <param name="content">The log content to be saved.</param>
        private void SaveLog(CSVType type, string content)
        {
            Logger.Log($"CSVManager::SaveLog - type: {type} | content: {content} | _coordinatePosFilePath: {_coordinatePosFilePath} | _summaryFilePath: {_summaryFilePath} | _coordinatesFilePath: {_coordinatesFilePath}");

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
        /// Initializes daily attempt counter.
        /// Triggered when entering a new day or session.
        /// </summary>
        private void InitDailyAttempt()
        {
            DailyAttempt = 0;

            var now = TimeManager.Instance.GetCurrentDateTime();
            var dayFolderPath = GetDayFolderPath(now, SettingManager.Instance.DisplayName);

            if (Directory.Exists(dayFolderPath))
            {
                // check existing session folders within day folder
                var sessionFolders = Directory.GetDirectories(dayFolderPath);
                foreach (var sessionFolder in sessionFolders)
                {
                    var sessionInfoFilePath = GetSessionInfoFilePath(sessionFolder);
                    var sessionAttempt = 0;

                    if (File.Exists(sessionInfoFilePath))
                    {
                        using (var reader = new StreamReader(sessionInfoFilePath))
                        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                        {
                            csv.Read();
                            csv.ReadHeader();

                            var infos = csv.GetRecords<CSVSessionInfo>();
                            foreach (var info in infos)
                            {
                                var calibrationNum = info.CalibrationNum;
                                var coreGameNum = info.CoreGameNum;
                                sessionAttempt = Mathf.Max(sessionAttempt, coreGameNum);
                            }
                        }
                    }

                    DailyAttempt += sessionAttempt;
                }
            }

            Logger.Log($"InitDailyAttempt - DailyAttempt: {DailyAttempt}");
        }

        /// <summary>
        /// Determines whether the maximum daily attempt is reached.
        /// </summary>
        /// <returns></returns>
        public bool IsDailyMaxAttemptReached()
        {
            var dailyAttempt = DailyAttempt;
            var dailyMaxAttempt = GameGeneralConfig.GetData().DailyMaxAttempt;
            var isDailyMaxAttemptReached = dailyAttempt >= dailyMaxAttempt;

            return isDailyMaxAttemptReached;
        }

        /// <summary>
        /// Starts a new logging session for either calibration or core game, and sets up the file paths for the logs.
        /// </summary>
        /// <param name="logTime">The timestamp indicating the start of the log.</param>
        /// <param name="isCoreGame">Indicates whether the log is for a core game session or calibration.</param>
        private bool StartLog(DateTime logTime, bool isCoreGame)
        {
            TryEndLog();

            if (SettingManager.Instance.IsDefaultUser)
            {
                Logger.Log($"CSVManager::StartLog - No log is generated for default user");
                return false;
            }

            var isSuccess = true;

            _isLogging = true;
            _isCoreGame = isCoreGame;

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
                var sessionNum = GetMaxSessionNumOfDay(logTime);

                if (_sessionNum == 0 || _sessionNum != sessionNum) // init session (folder and stuff)
                {
                    isNewSession = true;

                    _sessionNum = sessionNum + 1;
                }

                if (isNewSession)
                {
                    _sessionFolderPath = GetSessionFolderPath(dayFolderPath, _sessionNum, logTime);
                }

                Directory.CreateDirectory(_sessionFolderPath); // does nothing when already exists

                _coordinatePosFilePath = GetCoordinatePosFilePath(_sessionFolderPath, logTime, SettingManager.Instance.DisplayName, SettingManager.Instance.LevelTotal, SettingManager.Instance.LevelGoal, LevelManager.Instance.CurrentLevel);
                _coordinatesFilePath = GetCoordinatesFilePath(_sessionFolderPath, logTime, SettingManager.Instance.DisplayName, SettingManager.Instance.LevelTotal, SettingManager.Instance.LevelGoal, LevelManager.Instance.CurrentLevel);
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

        /// <summary>
        /// Gets the largest existing session number of the day.
        /// </summary>
        /// <param name="time">Current time as a DateTime object.</param>
        /// <returns>Retrieved largest session number.</returns>
        public int GetMaxSessionNumOfDay(DateTime time)
        {
            var sessionNum = 0;

            var dayFolderPath = GetDayFolderPath(time, SettingManager.Instance.DisplayName);

            if (Directory.Exists(dayFolderPath))
            {
                // check existing session folders within day folder
                var sessionFolders = Directory.GetDirectories(dayFolderPath);
                foreach (var sessionFolder in sessionFolders)
                {
                    if (TryParseSessionFolder(sessionFolder, out var pSessionNum))
                    {
                        sessionNum = Mathf.Max(sessionNum, pSessionNum);
                    }
                }
            }

            return sessionNum;
        }

        /// <summary>
        /// Generates a CSV header for a specific data type.
        /// </summary>
        /// <typeparam name="T">The type of the data (must implement ICSVData).</typeparam>
        /// <returns>A CSV header string.</returns>
        public string GenerateCSVHeader<T>() where T : ICSVData
        {
            using (var writer = new StringWriter())
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader(typeof(T));
                return writer.ToString();
            }
        }

        /// <summary>
        /// Generates CSV content from a list of data records.
        /// </summary>
        /// <typeparam name="T">The type of the data records.</typeparam>
        /// <param name="records">The data records to convert into CSV content.</param>
        /// <returns>A CSV content string.</returns>
        public string GenerateCSVContent<T>(IEnumerable<T> records) where T : ICSVData
        {
            using (var writer = new StringWriter())
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Appends a log entry to the appropriate log type (e.g., coordinates or summary).
        /// </summary>
        /// <param name="type">The type of log (e.g., Coordinates, Summary).</param>
        /// <param name="text">The text to append to the log.</param>
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

        #region Public
        /// <summary>
        /// Gets a list of directories of log storage for a specified player.
        /// </summary>
        /// <param name="username">Current player's username.</param>
        /// <returns>Retrieved list of log directories.</returns>
        public List<string> GetCSVDirectoriesForUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                Logger.LogError($"Invalid username: {username}");
                return null;
            }

            var directories = new List<string>();
            var allDirectories = Directory.GetDirectories(GetRootPath());
            foreach (var dir in allDirectories)
            {
                if (dir.Contains(username))
                {
                    directories.Add(dir);
                }
            }

            return directories;
        }
        #endregion
    }
}