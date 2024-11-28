using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotSpawner : MonoBehaviour
{
    public GameObject carrotPrefab; // The carrot prefab to spawn
    public float respawnDelay = 3f; // Delay before spawning a new carrot

    private GameObject currentCarrot; // Reference to the currently spawned carrot
    private float timer;

    private void Update()
    {
        // Check if the carrot is missing
        if (currentCarrot == null)
        {
            timer+=Time.deltaTime;
        }
        if (timer > respawnDelay)
        {
            SpawnCarrot();
        }
    }

    private void SpawnCarrot()
    {
        // If there's already a carrot, don't spawn another
        if (currentCarrot != null) return;

        // Instantiate a new carrot
        currentCarrot = Instantiate(carrotPrefab, transform.position, Quaternion.identity);
        timer = 0;
    }
}

