using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject meteoroidPrefab;
    public float maxX;
    public float maxY;
    public Transform spawnPoint;
    public float spawnRate = 2.0f; // Adjust the spawn rate as needed
    private bool gameStarted = false;
    public float meteoroidSpeed = 5.0f;

    private Camera mainCamera;
    private Vector2 screenBounds;

    // public int targetNumber;

    void Start()
    {
        // Start spawning meteoroids after a delay
        // StartCoroutine(StartSpawning());

        // genertate a random number form 1-6
    }

    void Update()
    {
        // You can add game-related logic here if needed
    }

    private IEnumerator StartSpawning()
    {
        yield return new WaitForSeconds(2.0f); // Delay before starting spawning

        gameStarted = true;

        while (gameStarted)
        {
            SpawnMeteoroid();
            yield return new WaitForSeconds(spawnRate);
        }
    }

    private void SpawnMeteoroid()
    {
        Vector3 spawnPos = spawnPoint.position;

        spawnPos.x = Random.Range(-maxX, maxX);
        spawnPos.y = Random.Range(-maxY, maxY);

        //make sure there is no exsiting meteorid 
        if(GameObject.Find("Meteorid(Clone)") == true) {
            GameObject meteoroid = Instantiate(meteoroidPrefab, spawnPos, Quaternion.identity);
            // Meteoroid meteoroidScript = meteoroid.GetComponent<Meteoroid>();
            // meteoroidScript.speed = meteoroidSpeed;
            
        }

    }
    public void StopSpawning()
    {
        gameStarted = false;
    }
}