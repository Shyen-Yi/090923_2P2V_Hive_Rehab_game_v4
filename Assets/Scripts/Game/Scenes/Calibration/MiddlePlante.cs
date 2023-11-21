using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public SpriteRenderer[] middle;
    public SpriteRenderer middleNormal;
    public SpriteRenderer middleCalibration;
    public SpriteRenderer middleDone;

    public SpriteRenderer spaceship;

    public float calibrationTimer = 0.0f;
    public float calibrationDuration = 4.0f;
    public bool isCollided = false;
    public int currentMiddleIndex = 0;

    void Start()
    {
        middle = new SpriteRenderer[] {
            middleNormal,
            middleCalibration,
            middleDone,
        };
        
        middle[currentMiddleIndex].enabled = true;

    }

    public void Update()
    {
        if(currentMiddleIndex == 0 && isCollided)
        {
            currentMiddleIndex = 1;
            calibrationTimer = 0.0f;
        }
        else if(currentMiddleIndex == 1 && calibrationTimer < calibrationDuration)
        {
            calibrationTimer += Time.deltaTime;
        }
        else if(currentMiddleIndex == 1 && calibrationTimer >= calibrationDuration)
        {
            currentMiddleIndex  = 2;
        }
        for(int i = 0; i < middle.Length ; i++) 
        {
            middle[i].enabled = i == currentMiddleIndex;
        }

    }
    
   
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Hand") && isCollided)
        {
            isCollided = true;
            Debug.Log("Hand collided!");
        }

    }   
}

