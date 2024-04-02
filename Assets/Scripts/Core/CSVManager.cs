using System.Collections;
using System.IO;
using UnityEngine;
using System.Text;

namespace com.hive.projectr
{
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

        #region Lifecycle
        public void OnInit()
        {
            MonoBehaviourUtil.OnUpdate += Tick;
        }

        public void OnDispose()
        {
            MonoBehaviourUtil.OnUpdate -= Tick;
        }
        #endregion

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

        public void StartRecording(Transform handObj)
        {
            _handObj = handObj;

            if (_csvWriter != null)
            {
                _isRecording = true;
                return;
            }

            // Get the current date and time
            System.DateTime currentTime = System.DateTime.Now;

            // Format the date and time as a string (you can adjust the format as needed)
            string dayString = currentTime.ToString("yyyy-MM-dd");
            string dateTimeString = currentTime.ToString("yyyy-MM-dd_HH-mm");

            // Define the folder path
#if UNITY_EDITOR
            string folderPath = Path.Combine(Application.dataPath, "CSVFiles", dayString);
#else
            string folderPath = Path.Combine(Application.persistentDataPath, "CSVFiles", dayString);
#endif

            // Create the directory if it doesn't exist
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Define the file path for the CSV file with the date and time in the name
            _filePath = folderPath + $"/movement_data_{dateTimeString}.csv";

            // Create or overwrite the CSV file
            _csvWriter = new StreamWriter(_filePath, false);

            _logSb = new StringBuilder();

            AddLine("Time,CursorX,CursorY,MeteorX,MeteorY");
            AddLine(string.Format("{0:yyyy-MM-dd HH:mm:ss.fff},{1:F2},{2:F2},{3:F2},{4:F2}", currentTime, 0, 0, 0, 0));

            // Start recording
            _isRecording = true;
        }

        private void AddLine(string text)
        {
            _logSb.AppendLine(text);
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
    }
}