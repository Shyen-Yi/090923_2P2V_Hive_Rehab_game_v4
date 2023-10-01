using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject meteoroidPrefab;
    public GameManagerScriptableObject gameManager;
    //public float maxX;
    //public float maxY;
    //public Transform spawnPoint;
    //public float spawnRate = 2.0f; // Adjust the spawn rate as needed
    private bool gameStarted = false;
    //public float meteoroidSpeed = 5.0f;
    public float instantiationInterval = 5f;
    private float timer = 0f;

    void Start()
    {
    }

    void Update()
    {
        // You can add game-related logic here if needed
        timer += Time.deltaTime;

        if (timer >= instantiationInterval)
        {
            InstantiatePrefab();
            timer = 0f;
        }
    }

    private void InstantiatePrefab()
    {
        if (GameObject.Find("Meteorid(Clone)") == null)
        {
            // Prefab doesn't exist, so instantiate it
            Instantiate(meteoroidPrefab, transform.position, Quaternion.identity);
            gameManager.isGameRunning = true;
        }
        else
        {
            // Prefab already exists
            Debug.Log("Prefab already exists.");
        }
    }
}