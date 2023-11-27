using System.Xml.Schema;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HandMovement : MonoBehaviour
{
    public SpriteRenderer spaceship;
    public Sprite spaceshipOn;
    public Sprite spaceshipOff;
    public SpriteRenderer spaceshipHover;
    public SpriteRenderer spaceshipSnapObject;

    public GameObject Asteroid;
    
    // define initial state
    public float timer = 0f;
    public bool isActive = false;


    // define Hover state
    public bool isHoverActive = false;
    public bool isMouseMoving = false;

    // define Snap state
    public bool isSnapped = false;

    // define boundary value
    public float minX = -68.3f;
    public float maxX = 68.3f;
    public float minY = -28.6f;
    public float maxY = 28.6f;

    private void Start()
    {
        spaceship.sprite = spaceshipOff;
        StartCoroutine (startSpaceshipSwapping());
    }

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Cursor.visible = false;
        mousePosition.z = 0f;

        // this.transform.position = mousePosition;

        float clampedX = Mathf.Clamp(mousePosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(mousePosition.y, minY, maxY);
        Vector3 clampedMousePosition = new Vector3(clampedX, clampedY, 0f);

        transform.position = clampedMousePosition;

        // Debug.Log(clampedX);
        // Debug.Log(clampedY);

        float mouseDelta = Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y");
        isMouseMoving = Mathf.Abs(mouseDelta) > 0.01f;
    }

    private IEnumerator startSpaceshipSwapping()
    {
        while(true)
        {
            if(isMouseMoving && isHoverActive)
            {
                spaceship.sprite = spaceshipHover.sprite;
                isHoverActive = true;
                isSnapped = false;
            }
            else
            {
                spaceship.sprite = isActive ? spaceshipOff : spaceshipOn;
            }
            // isActive = !isActive;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void turnOffHoverEvent()
    {
        isHoverActive = false;
    }

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        if(collider.gameObject.CompareTag("Meteoroid") && !isSnapped)
        {
            spaceship.sprite = spaceshipSnapObject.sprite;
            isSnapped = true;
            // turnOffHoverEvent();
            Debug.Log("Collision detected with Meteoroid");
        }
        // else 
        // {
        //     spaceship.sprite = spaceshipHover.sprite;
        //     Debug.Log("No Collision with Meteroid detected");
        // }
    }
}

