using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float followSpeed = 5.0f;
    private float screenWidth;
    private float screenHeight;
    private bool cursorHidden = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get the screen dimensions
        screenWidth = 1366.0f;
        screenHeight = 768.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            cursorHidden = !cursorHidden;
            Cursor.visible = !cursorHidden;
        }
        // Get the current mouse position in world coordinates
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Ensure the object stays at the same Z-coordinate to avoid depth issues
        mousePosition.z = transform.position.z;

        // Calculate the direction from the object to the mouse cursor
        Vector3 direction = (mousePosition - transform.position).normalized;

        // Calculate the movement based on the direction and speed
        Vector3 movement = direction * followSpeed * Time.deltaTime;

        // Calculate the object's position after movement
        Vector3 newPosition = transform.position + movement;

        // Ensure the object stays within the screen boundaries
        newPosition.x = Mathf.Clamp(newPosition.x, -screenWidth / 2, screenWidth / 2);
        newPosition.y = Mathf.Clamp(newPosition.y, -screenHeight / 2, screenHeight / 2);

        // Apply the new position to the object
        transform.position = newPosition;

    }

}