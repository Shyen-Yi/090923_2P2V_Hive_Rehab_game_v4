using UnityEngine;

public class MeteoroidMovement : MonoBehaviour
{
    public float speed = 5.0f;
    private Vector3 moveDirection; // Store the random movement direction
    private bool hasCollided = false;
    private Transform otherTransform;
    private bool followMouse = false; // Flag to indicate if the meteoroid should follow the mouse

    [SerializeField]
    private float smoothMovementAmount = 5f; // Adjust this value to control the smoothness of the movement

    void Start()
    {
        // Choose a random direction when the meteoroid is created
        moveDirection = GetRandomDirection();
    }

    void Update()
    {
        if (hasCollided)
        {
            if (otherTransform != null)
            {
                // Smoothly interpolate the position of the meteoroid towards the mouse position
                transform.position = Vector3.Lerp(transform.position, otherTransform.position, smoothMovementAmount * Time.deltaTime);
                if ((transform.position - otherTransform.position).magnitude < 0.5f)
                {
                    followMouse = true;
                }
            }
        }
        if (followMouse)
        {
            transform.position = otherTransform.position;
        }
        else
        {
            // Move the meteoroid in the chosen random direction
            transform.Translate(moveDirection * speed * Time.deltaTime);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (hasCollided) return;

        if (collision.CompareTag("Hand"))
        {
            otherTransform = collision.transform;
            hasCollided = true;
        }
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