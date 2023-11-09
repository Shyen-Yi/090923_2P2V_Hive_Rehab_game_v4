using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoroidDestroyer : MonoBehaviour
{

    private void Update()
    {
        // Check if the object is out of the screen view
        if (!IsVisibleOnScreen())
        {
            // Lower Score Here
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if the object collided with an object tagged as "Bucket"

        if (collider.CompareTag("Bucket"))
        {
            // Increase Score Here
            Destroy(gameObject);
        }
    }

    // Function to check if the object is within the camera's view
    private bool IsVisibleOnScreen()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        return (screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1);
    }

}