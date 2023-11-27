using System.Collections;
using System.IO;
using UnityEngine;

namespace com.hive.projectr
{
    public class CSVManager : SingletonBase<CSVManager>, ICoreManager
    {
        [SerializeField]
        private Transform handObject; // Reference to the GameObject whose position we want to record

        private GameObject meteorObject = null; //Is Meteoroid correct? Sounds weird

        private string filePath;
        private StreamWriter csvWriter;
        [SerializeField]
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
                if (handObject == null)
                {
                    StopRecording();
                    return;
                }

                if (Time.time >= _nextWriteTime)
                {
                    meteorObject = GameObject.FindWithTag("Meteoroid");

                    if (meteorObject == null)
                    {
                        // Record the current position and time with timestamp
                        string line = string.Format("{0:yyyy-MM-dd HH:mm:ss.fff},{1:F2},{2:F2}",
                            System.DateTime.Now, handObject.position.x, handObject.position.y);

                        // Write the line to the CSV file
                        csvWriter.WriteLine(line);
                    }
                    else
                    {
                        // Handle the case where no object with the "Meteoroid" tag was found
                        // Debug.LogWarning("No object with the 'Meteoroid' tag was found.");
                        string line = string.Format("{0:yyyy-MM-dd HH:mm:ss.fff},{1:F2},{2:F2},{3:F2},{4:F2},",
                            System.DateTime.Now, handObject.position.x, handObject.position.y,
                                                 meteorObject.transform.position.x, meteorObject.transform.position.y);

                        // Write the line to the CSV file
                        csvWriter.WriteLine(line);
                    }

                    // Wait for the specified interval
                    _nextWriteTime = Time.time + recordInterval;
                }
            }
        }

        public void StartRecording()
        {
            // Get the current date and time
            System.DateTime currentTime = System.DateTime.Now;

            // Format the date and time as a string (you can adjust the format as needed)
            string dateTimeString = currentTime.ToString("yyyy-MM-dd_HH-mm");

            // Define the folder path
            string folderPath = Application.dataPath + "/CSVFiles/";

            // Create the directory if it doesn't exist
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Define the file path for the CSV file with the date and time in the name
            filePath = folderPath + "movement_data_" + dateTimeString + ".csv";

            // Create or overwrite the CSV file
            csvWriter = new StreamWriter(filePath, false);

            // Write headers to the CSV file
            csvWriter.WriteLine("Time,CursorX,CursorY,MeteorX,MeteorY");
            csvWriter.WriteLine(string.Format("{0:yyyy-MM-dd HH:mm:ss.fff},{1:F2},{2:F2},{3:F2},{4:F2}",
                 currentTime, 0, 0, 0, 0));

            // Start recording
            _isRecording = true;
        }

        public void StopRecording()
        {
            _isRecording = false;

            // Close the CSV file when recording is terminated
            if (csvWriter != null)
            {
                csvWriter.Close();
                csvWriter = null;
            }
        }
    }
}