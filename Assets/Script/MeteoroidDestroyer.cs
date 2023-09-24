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
            Destroy(gameObject);
        }

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object collided with an object tagged as "Bucket"

        Debug.Log(other);
        if (other.CompareTag("Bucket"))
        {
            Destroy(gameObject);
        }
    }

    // Function to check if the object is within the camera's view
    private bool IsVisibleOnScreen()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        return (screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1);
    }
    //void OnTriggerEnter2D(Collider2D collision)
    //{
    //    // Destroy meteoroids when they leave a certain area
    //    if (collision.CompareTag("Meteoroid"))
    //    {
    //        Destroy(collision.gameObject);
    //    }
    //}
}