using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteoroid : MonoBehaviour
{
    public float speed = 5.0f;  // Speed at which the meteoroid moves
    private bool isMoving = false;  // Flag to track if the meteoroid is moving

    // Start is called before the first frame update
    void Start()
    {
        // Start the meteoroid's movement when it's created
        StartMoving();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the meteoroid has moved out of the screen bounds
        if (isMoving && IsOutOfScreenBounds())
        {
            // Destroy the meteoroid when it's out of bounds
            Destroy(gameObject);
        }
    }

    public void StartMoving()
    {
        // Apply a random velocity to the meteoroid
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        // Randomly choose a direction using Random.Range
        float randomX = Random.Range(-1.0f, 1.0f); // Random X component between -1 and 1
        float randomY = Random.Range(-1.0f, 1.0f); // Random Y component between -1 and 1

        // Create a random velocity vector
        Vector2 randomVelocity = new Vector2(randomX, randomY).normalized * speed;

        // Apply the random velocity to the meteoroid
        rb.velocity = randomVelocity;

        // Set the flag to indicate that the meteoroid is moving
        isMoving = true;
    }

    // Method to check if the meteoroid is out of screen bounds
    private bool IsOutOfScreenBounds()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        return screenPoint.y > 1.1f;  // Adjust this threshold as needed
    }

    // This method is called when a collision occurs
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision occurred with a collider (e.g., a bound)
        // Destroy the meteoroid immediately upon collision
        Destroy(gameObject);
    }
}