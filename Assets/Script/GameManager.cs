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
    public float instantiationInterval = 5f;
    private float timer = 0f;

    void Start()
    {
        // Start spawning meteoroids after a delay
        //StartCoroutine(StartSpawning());
        InstantiatePrefab();
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
        }
        else
        {
            // Prefab already exists
            Debug.Log("Prefab already exists.");
        }
    }

    /*
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

        GameObject meteoroid = Instantiate(meteoroidPrefab, spawnPos, Quaternion.identity);

        
        //Meteoroid meteoroidScript = meteoroid.GetComponent<Meteoroid>();
        //meteoroidScript.speed = meteoroidSpeed;
    }

    // You can use this method to stop spawning meteoroids
    public void StopSpawning()
    {
        gameStarted = false;
    }

    */
}