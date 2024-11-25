using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingCarrots : MonoBehaviour
{
    public float speed = 5f;              // Movement speed of the carrot
    public float rotationSpeed = 200f;   // How fast the carrot adjusts its direction
    public float detectionRadius = 10f;  // Detection range for the player
    public float arcFactor = 1f;         // Controls how much the carrot swerves in its arc

    private Transform player;
    private bool playerDetected = false;

    void Start()
    {
        // Slightly randomize the initial direction for an arc
        Vector2 randomDirection = new Vector2(Random.Range(-arcFactor, arcFactor), Random.Range(-arcFactor, arcFactor)).normalized;
        transform.up += new Vector3(randomDirection.x, randomDirection.y, 0);
    }

    void Update()
    {
        DetectPlayer(); // Dynamically check if the player is within range

        if (playerDetected && player != null)
        {
            HomeInOnPlayer(); // Move and home in on the player
        }
        else
        {
            // Stay still when no player is detected
            StayStill();
        }
    }

    void DetectPlayer()
    {
        // Search for the player within the detection radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player")) // Ensure the player GameObject is tagged "Player"
            {
                player = hit.transform;
                playerDetected = true;
                return;
            }
        }

        // If no player is found
        playerDetected = false;
    }

    void HomeInOnPlayer()
    {
        // Calculate the direction to the player
        Vector2 currentDirection = transform.up;
        Vector2 desiredDirection = (player.position - transform.position).normalized;

        // Gradually rotate towards the desired direction
        float angle = Vector3.SignedAngle(currentDirection, desiredDirection, Vector3.forward);
        float rotationStep = rotationSpeed * Time.deltaTime;
        float adjustedAngle = Mathf.Clamp(angle, -rotationStep, rotationStep);

        // Apply rotation
        transform.Rotate(Vector3.forward, adjustedAngle);

        // Move forward in the current direction
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
    }

    void StayStill()
    {
        // Simply do nothing to keep the carrot in place
    }

    // Visualize detection radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
