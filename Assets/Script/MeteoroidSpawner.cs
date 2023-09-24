using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoroidSpawner : MonoBehaviour
{
    public GameObject meteoroidPrefab;
    public Transform spawnPoint;
    public float spawnRate = 2.0f; // Adjust the spawn rate as needed
    private bool gameStarted = false;

    void Start()
    {
        // Start spawning meteoroids after a delay
        StartCoroutine(StartSpawning());
    }

    private IEnumerator StartSpawning()
    {
        yield return new WaitForSeconds(5.0f); // Delay before starting spawning

        gameStarted = true;

        while (gameStarted)
        {
            Vector3 spawnPos = spawnPoint.position;
            GameObject meteoroidObject = GameObject.FindWithTag("Meteoroid");

            Debug.Log(meteoroidObject);

            if (meteoroidObject == null)
            {
                GameObject meteoroid = Instantiate(meteoroidPrefab, spawnPos, Quaternion.identity);
            }
                
            yield return new WaitForSeconds(spawnRate);
        }
    }

    // You can use this method to stop spawning meteoroids
    public void StopSpawning()
    {
        gameStarted = false;
    }
}