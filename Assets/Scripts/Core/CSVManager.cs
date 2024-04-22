using System.Collections;
using System.IO;
using UnityEngine;
using System.Text;
using System;

namespace com.hive.projectr
{
    #region CSV Data
    public struct CSVAsteroid
    {
        public int asteroidId;
        public float spawnTime;
        public Vector2 asteroidCoordinateWhenSpawned;
        public Vector2 cursorCoordinateWhenAsteroidSpawned;
        public float captureSec;
        public Vector2 asteroidCoordinateWhenCaptured;
        public Vector2 cursorCoordinateWhenAsteroidCaptured;
        public bool isAsteroidCaptured;
        public bool isAsteroidDestroyed;
        public Vector2 vacuumCenterCoordinate;
    }

    public struct CSVCalibration
    {
        public Vector2 center;
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomRight;
        public Vector2 bottomLeft;

        public CSVCalibration(Vector2 center, Vector2 topLeft, Vector2 topRight, Vector2 bottomRight, Vector2 bottomLeft)
        {
            this.center = center;
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
            this.bottomLeft = bottomLeft;
        }
    }
    #endregion

    public class CSVManager : SingletonBase<CSVManager>, ICoreManager
    {
        private int _calibrationNum;
        private int _coreGameNum;
        private int _sessionNum;
        private DateTime _logTime;
        private StringBuilder _logTextSb;

        private static readonly string DayFolderTemplate = "{0:00}/{1:00}/{2:0000}.{3}"; // 03/13/2024.Mark
        private static readonly string SessionFolderTemplate = SessionNumPrefix + "{0}.{1:00}:{2:00}"; // Session#3.14:25
        private static readonly string SessionNumPrefix = "Session#";
        private static readonly string CoordinatePosFileTemplate = "{0}_{1}/{2}/{3}_{4}Block_Level{5}_CoordinatePOS"; // Mark_03/13/2024_100Block_Level1_CoordinatePOS
        private static readonly string CoordinatesFileTemplate = "{0}_{1}/{2}/{3}_{4}Block_Level{5}_Coordinates"; // Mark_03/13/2024_100Block_Level1_Coordinates
        private static readonly string SummaryFileTemplate = "{0}_Summary"; // Mark_Summary




        private Transform _handObj; // Reference to the GameObject whose position we want to record
        private GameObject _asteroidObj = null; //Is Meteoroid correct? Sounds weird

        private string _filePath;
        private StreamWriter _csvWriter;
        private StringBuilder _logSb;
        private float recordInterval = 0.1f; // Record position every 0.1 second

        private bool _isRecording;
        private float _nextWriteTime;

        #region Lifecycle
        public void OnInit()
        {
            SetupDayFolder(TimeUtil.Now);

            MonoBehaviourUtil.OnUpdate += Tick;
        }

        public void OnDispose()
        {
            MonoBehaviourUtil.OnUpdate -= Tick;
        }
        #endregion

        #region Calibration
        public void OnCalibrationStarted(DateTime logTime)
        {
            Init(logTime);
        }

        /// <summary>
        /// Called when a calibration is finished.
        /// Save calibration CSV data.
        /// </summary>
        /// <param name="data"></param>
        public void OnCalibrationEnded(CSVCalibration data)
        {


            Reset();
        }
        #endregion

        #region Core Game
        public void OnCoreGameStarted(DateTime logTime)
        {
            Init(logTime);
            AppendLog($"Asteroid's ID, Asteroid Spawn Time, Asteroid's Coordinate X (Spawned), Asteroid's Coordinate Y (Spawned), Cursor's Coordinate X (Spawned), Cursor's Coordinate Y (Spawned), Is Asteroid Captured, Asteroid Capture Time, Asteroid's Coordinate X (Captured), Asteroid's Coordinate Y (Captured), Cursor's Coordinate X (Captured), Cursor's Coordinate Y (Captured), Is Asteroid Destroyed, Asteroid Destroy Time, Asteroid's Coordinate X (Destroyed), Asteroid's Coordinate Y (Destroyed), Vacuum's Coordinate X (Destroyed), Vacuum's Coordinate Y (Destroyed)");
        }

        public void OnCoreGameAsteroidSpawned(CSVAsteroid data)
        {
        }

        public void OnCoreGameEnded()
        {


            Reset();
        }
        #endregion

        #region Private
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
        /// Need to be called before writing any log to local file
        /// </summary>
        /// <param name="logTime">Generation time of the log</param>
        /// <returns>If success</returns>
        private bool Init(DateTime logTime)
        {
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

                /*
                var coordinatePosFilePath = GetCoordinatePosFilePath(sessionFolderPath, time, SettingManager.Instance.DisplayName, SettingManager.Instance.DailyBlock, SettingManager.Instance.Level);
                var coordinatesFilePath = GetCoordinatesFilePath(sessionFolderPath, time, SettingManager.Instance.DisplayName, SettingManager.Instance.DailyBlock, SettingManager.Instance.Level);
                var summaryFilePath = GetSummaryFilePath(sessionFolderPath, SettingManager.Instance.DisplayName);
                */
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return false;
            }

            Logger.Log($"CSVManager::Setup - _sessionNum: {_sessionNum} | _calibrationNum: {_calibrationNum} | _coreGameNum: {_coreGameNum}");
            return true;
        }

        private void Reset()
        {
            _calibrationNum = 0;
            _coreGameNum = 0;
            _sessionNum = 0;
            _logTime = DateTime.MinValue;
            _logTextSb = null;
        }

        private void AppendLog(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (_logSb != null)
            {
                _logSb.Append(text);
            }
            else
            {
                Logger.LogError($"Cannot append log when _logSb is null!");
            }
        }
        #endregion

        #region Public
        public void StartRecording(Transform handObj)
        {
            _handObj = handObj;

            if (_csvWriter != null)
            {
                _isRecording = true;
                return;
            }

            // Get the current date and time
            var currentTime = TimeUtil.Now;

            // Format the date and time as a string (you can adjust the format as needed)
            var dateTimeString = currentTime.ToString("yyyy-MM-dd_HH-mm");

            // Day folder
            var dayFolderPath = SetupDayFolder(currentTime);

            // Session folder
            var maxSessionNum = 0;
            var sessionFolders = Directory.GetDirectories(dayFolderPath);
            var sessionFolderPrefix = "Session#";
            foreach (var sessionFolder in sessionFolders)
            {
                var sessionStr = sessionFolder.Substring(sessionFolder.IndexOf(sessionFolderPrefix) + sessionFolderPrefix.Length);
                if (sessionStr.Length > 0 &&
                    int.TryParse(sessionStr[0].ToString(), out var sessionNum))
                {
                    maxSessionNum = Mathf.Max(maxSessionNum, sessionNum);
                }
                else
                {
                    Debug.LogError($"Invalid session folder name detected! Please check! -> {sessionFolder}");
                }
            }

            var newSessionNum = maxSessionNum + 1;
            var sessionFoliderPath = Path.Combine(dayFolderPath, $"{sessionFolderPrefix}{newSessionNum}.{currentTime.Hour:00}:{currentTime.Minute:00}");

            if (!Directory.Exists(sessionFoliderPath))
            {
                Directory.CreateDirectory(sessionFoliderPath);
            }

            var sessionCoordinates1Path = Path.Combine(sessionFoliderPath, $"{SettingManager.Instance.DisplayName}_{currentTime.Month:00}/{currentTime.Day:00}/{currentTime.Year:0000}_Block{SettingManager.Instance.DailyBlock}_Level{SettingManager.Instance.Level}_CoordinatePOS");
            var sessionCoordinates2Path = Path.Combine(sessionFoliderPath, $"{SettingManager.Instance.DisplayName}_{currentTime.Month:00}/{currentTime.Day:00}/{currentTime.Year:0000}_Block{SettingManager.Instance.DailyBlock}_Level{SettingManager.Instance.Level}_Coordinate");
            var sessionSummaryPath = Path.Combine(sessionFoliderPath, $"{SettingManager.Instance.DisplayName}_{currentTime.Month:00}/{currentTime.Day:00}/{currentTime.Year:0000}_Block{SettingManager.Instance.DailyBlock}_Level{SettingManager.Instance.Level}_Summary");

            // Define the file path for the CSV file with the date and time in the name
            _filePath = dayFolderPath + $"/movement_data_{dateTimeString}.csv";

            // Create or overwrite the CSV file
            _csvWriter = new StreamWriter(_filePath, false);

            _logSb = new StringBuilder();

            AddLine("Time,CursorX,CursorY,MeteorX,MeteorY");
            AddLine(string.Format("{0:yyyy-MM-dd HH:mm:ss.fff},{1:F2},{2:F2},{3:F2},{4:F2}", currentTime, 0, 0, 0, 0));

            // Start recording
            _isRecording = true;
        }

        public void PauseRecording()
        {
            _isRecording = false;
            AddLine("######################## PAUSED ########################");
        }

        public void StopRecording()
        {
            _isRecording = false;

            // Close the CSV file when recording is terminated
            if (_csvWriter != null)
            {
                if (_logSb != null)
                {
                    _csvWriter.Write(_logSb.ToString());
                }

                _logSb = null;

                _csvWriter.Close();
                _csvWriter = null;
            }
        }
        #endregion

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

        private void Tick()
        {
            if (_isRecording)
            {
                if (_handObj == null)
                {
                    StopRecording();
                    return;
                }

                if (Time.time >= _nextWriteTime)
                {
                    _asteroidObj = GameObject.FindWithTag(TagNames.Asteroid);

                    if (_asteroidObj == null)
                    {
                        // Record the current position and time with timestamp
                        string line = string.Format("{0:yyyy-MM-dd HH:mm:ss.fff},{1:F2},{2:F2}",
                            System.DateTime.Now, _handObj.position.x, _handObj.position.y);

                        // Write the line to the CSV file
                        AddLine(line);
                    }
                    else
                    {
                        // Handle the case where no object with the "Meteoroid" tag was found
                        // Debug.LogWarning("No object with the 'Meteoroid' tag was found.");
                        string line = string.Format("{0:yyyy-MM-dd HH:mm:ss.fff},{1:F2},{2:F2},{3:F2},{4:F2},",
                            System.DateTime.Now, _handObj.position.x, _handObj.position.y,
                                                 _asteroidObj.transform.position.x, _asteroidObj.transform.position.y);

                        // Write the line to the CSV file
                        AddLine(line);
                    }

                    // Wait for the specified interval
                    _nextWriteTime = Time.time + recordInterval;
                }
            }
        }

        private void AddLine(string text)
        {
            _logSb.AppendLine(text);
        }
    }
}