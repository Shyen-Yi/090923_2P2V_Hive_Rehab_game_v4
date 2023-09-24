using UnityEngine;

public class MeteoroidMovement : MonoBehaviour
{
    public float speed = 5.0f;
    private Vector3 moveDirection; // Store the random movement direction
    private bool followMouse = false; // Flag to indicate if the meteoroid should follow the mouse

    void Start()
    {
        // Choose a random direction when the meteoroid is created
        moveDirection = GetRandomDirection();
    }

    void Update()
    {
        if (followMouse)
        {
            // Get the mouse position in world space
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Set the position of the meteoroid to follow the mouse
            transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
        }
        else
        {
            // Move the meteoroid in the chosen random direction
            transform.Translate(moveDirection * speed * Time.deltaTime);
        }
    }

    void OnMouseEnter()
    {
        // When the mouse enters the meteoroid's collider, stop random movement and follow the mouse
        followMouse = true;
    }

    void OnMouseExit()
    {
        // When the mouse exits the meteoroid's collider, resume random movement
        followMouse = false;
        // Choose a new random direction
        moveDirection = GetRandomDirection();
    }

    Vector3 GetRandomDirection()
    {
        // Generate a random integer between 0 and 3 to choose a direction
        int randomDirectionIndex = Random.Range(0, 4);

        // Initialize a vector for each possible direction
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };

        // Return the selected random direction
        return directions[randomDirectionIndex];
    }
}