using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hive.projectr
{
    public class CalibrationController : GameSceneControllerBase
    {
        #region Fields
        private Vector2[] calibrationCorners = new Vector2[4]; // To store the calibrated corners
        private int calibrationCornerIndex = 0;
        private bool isCalibrating = false;
        private float calibrationTime = 3f; // Time required to hold a corner for calibration
        private float calibrationTimer = 0f;
        private bool isCalibrated = false;
        #endregion

        #region Extra
        private enum ExtraGO
        {
            MarkPrefab = 0,
        }

        private GameObject _markPrefab;
        #endregion

        #region Lifecycle
        protected override void OnInit()
        {
            InitExtra();
            BindActions();
        }

        private void InitExtra()
        {
            _markPrefab = Config.ExtraGameObjects[(int)ExtraGO.MarkPrefab];
        }

        protected override void OnShow(ISceneData data)
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnDispose()
        {
            UnbindActions();
        }
        #endregion

        #region UI Binding
        private void BindActions()
        {
            MonoBehaviourUtil.OnUpdate += Tick;
        }

        private void UnbindActions()
        {
            MonoBehaviourUtil.OnUpdate -= Tick;
        }
        #endregion

        #region Callback
        private void Tick()
        {
            if (!isCalibrated)
            {
                if (isCalibrating)
                {
                    calibrationTimer += Time.deltaTime;
                    if (calibrationTimer >= calibrationTime)
                    {
                        calibrationCorners[calibrationCornerIndex] = Input.mousePosition;
                        calibrationCornerIndex++;
                        if (calibrationCornerIndex >= 4)
                        {
                            // All corners are calibrated
                            isCalibrated = true;
                            isCalibrating = false;
                        }
                        else
                        {
                            calibrationTimer = 0f;
                        }
                    }
                }
                else
                {
                    // Check if the player wants to calibrate
                    if (Input.GetMouseButtonDown(0))
                    {
                        isCalibrating = true;
                        calibrationTimer = 0f;
                    }
                }
            }
            else
            {
                // Calculate the calibrated screen dimensions
                float minX = Mathf.Min(calibrationCorners[0].x, calibrationCorners[2].x);
                float minY = Mathf.Min(calibrationCorners[1].y, calibrationCorners[3].y);
                float maxX = Mathf.Max(calibrationCorners[0].x, calibrationCorners[2].x);
                float maxY = Mathf.Max(calibrationCorners[1].y, calibrationCorners[3].y);

                // Apply these dimensions to determine the allowed gameplay area
                Rect allowedArea = new Rect(minX, minY, maxX - minX, maxY - minY);

                // Check if the mouse is within the allowed gameplay area
                Vector2 mousePosition = Input.mousePosition;
                if (allowedArea.Contains(mousePosition))
                {
                    // You can perform mouse-marking logic here if needed
                    if (Input.GetMouseButtonDown(0))
                    {
                        // Instantiate a mark at the mouse position
                        GameObject.Instantiate(_markPrefab, mousePosition, Quaternion.identity);
                    }
                }
            }
        }
        #endregion
    }
}