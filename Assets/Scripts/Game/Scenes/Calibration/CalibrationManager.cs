using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationManager : MonoBehaviour

{
    private Vector2[] calibrationCorners = new Vector2[4]; // To store the calibrated corners
    private int calibrationCornerIndex = 0;
    private bool isCalibrating = false;
    private float calibrationTime = 3f; // Time required to hold a corner for calibration
    private float calibrationTimer = 0f;
    private bool isCalibrated = false;

    public GameObject middle;
    public GameObject TopRight;
    public GameObject TopLeft;
    public GameObject BottomLeft;
    public GameObject BottomRight;

    public GameObject ReminderText;

    public GameObject markPrefab;

    public void Update()
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
                    Instantiate(markPrefab, mousePosition, Quaternion.identity);
                }
            }
        }
    }
}