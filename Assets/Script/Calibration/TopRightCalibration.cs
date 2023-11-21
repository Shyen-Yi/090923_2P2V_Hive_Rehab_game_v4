using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class TopRightCalibration : MonoBehaviour
{
    public SpriteRenderer TopRightDisplay;
    public SpriteRenderer TopRightNormal;
    public SpriteRenderer TopRightCalibrating;
    public SpriteRenderer TopRightDone;

    public float calibrationCompleted = 5.0f;
    public float calibrationInitialTimer = 0.0f;
    public float calibrationDuration = 2.0f;

    public bool isMouseMoving = false;
    public bool isCalibrating = false;
    public bool isCalibrated = false;

    private Vector2 calibrationStartPosition;

    
    void Start()
    {
        TopRightDisplay.enabled = false;
    }

    void Update()
    {
        float mouseDelta = Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y");
        isMouseMoving = Mathf.Abs(mouseDelta) > 0.01f;
        isMouseMoving = true;

        if (isMouseMoving)
        {
            calibrationInitialTimer += Time.deltaTime;
            

            if (calibrationInitialTimer >= calibrationDuration && !isCalibrating)
            {
                TopRightDisplay.enabled = true;
                isCalibrating = true;
                isMouseMoving = false;
                TopRightDisplay.sprite = TopRightCalibrating.sprite;
                calibrationStartPosition = transform.position; // Store the starting position for calibration
                Debug.Log("Update called");
            }

            if (calibrationInitialTimer >= calibrationCompleted && !isCalibrated)
            {
                isCalibrating = false;
                isCalibrated = true;
                TopRightDisplay.sprite = TopRightDone.sprite;
                // Mark the calibration completion location using calibrationStartPosition
                Debug.Log("Calibration Completed at: " + calibrationStartPosition);
            }
        }
        else
        {
            calibrationInitialTimer = 0.0f; // Reset the timer if mouse is not moving
        }

        // Check if the user moves while calibrating or calibrated, cancel the calibration
        if ((isCalibrating || isCalibrated) && isMouseMoving)
        {
            ResetCalibration();
        }
    }

    private void ResetCalibration()
    {
        isMouseMoving = false;
        isCalibrating = false;
        isCalibrated = false;
        TopRightDisplay.sprite = TopRightNormal.sprite;
        calibrationInitialTimer = 0.0f;
        Debug.Log("Calibration Cancelled");
    }
}