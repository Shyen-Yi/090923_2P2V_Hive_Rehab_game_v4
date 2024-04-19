using System.Collections;
using System.IO;
using UnityEngine;
using System.Text;
using System;

namespace com.hive.projectr
{
    #region CSV Data
    public struct CSVCalibrationData
    {
        public Vector2 center;
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomRight;
        public Vector2 bottomLeft;

        public CSVCalibrationData(Vector2 center, Vector2 topLeft, Vector2 topRight, Vector2 bottomRight, Vector2 bottomLeft)
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




        private Transform _handObj; // Reference to the GameObject whose position we want to record
        private GameObject _asteroidObj = null; //Is Meteoroid correct? Sounds weird

        private string _filePath;
        private StreamWriter _csvWriter;
        private StringBuilder _logSb;
        private float recordInterval = 0.1f; // Record position every 0.1 second

        private bool _isRecording;
        private float _nextWriteTime;
        private int _sessionNum;

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

        #region Private
        private void Setup()
        {
            var time = TimeUtil.Now;
            var dayFolderPath = "";

            try
            {
                var dayString = $"{time.ToString("yyyy-MM-dd")}.";

#if UNITY_EDITOR
                dayFolderPath = Path.Combine(Application.dataPath, "CSVFiles", $"{dayString}.{SettingManager.Instance.DisplayName}");
#else
                dayFolderPath = Path.Combine(Application.persistentDataPath, "CSVFiles", $"{dayString}.{SettingManager.Instance.DisplayName}");
#endif

                if (!Directory.Exists(dayFolderPath))
                {
                    Directory.CreateDirectory(dayFolderPath);
                }

            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }
        #endregion

        #region Public



        public void RecordCalibrationData(CSVCalibrationData data)
        {
            var dayFolderPath = SetupDayFolder(TimeUtil.Now);

            _csvWriter = new StreamWriter(_filePath, false);
            _filePath = dayFolderPath + $"/movement_data_{dateTimeString}.csv";

        }

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
            var dayFolderPath = "";

            try
            {
                var dayString = $"{time.ToString("yyyy-MM-dd")}.";

#if UNITY_EDITOR
                dayFolderPath = Path.Combine(Application.dataPath, "CSVFiles", $"{dayString}.{SettingManager.Instance.DisplayName}");
#else
                dayFolderPath = Path.Combine(Application.persistentDataPath, "CSVFiles", $"{dayString}.{SettingManager.Instance.DisplayName}");
#endif

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