using UnityEngine;

public class CameraAutoScroll : MonoBehaviour
{
    public Transform player;           // Reference to the player object
    public float scrollSpeed = 2f;     // Speed at which the camera scrolls vertically
    private Camera mainCamera;         // Reference to the Main Camera

    void Start()
    {
        mainCamera = Camera.main;      // Get the main camera reference
    }

    void Update()
    {
        // Auto-scroll the camera vertically
        AutoScroll();

        // Check if the player touches the camera's borders
        CheckPlayerCollisionWithCameraBorder();
    }

    void AutoScroll()
    {
        // Move the camera down at a constant speed (along the Y-axis)
        transform.position += new Vector3(0, -scrollSpeed * Time.deltaTime, 0);
    }

    void CheckPlayerCollisionWithCameraBorder()
    {
        // Get the camera's world space edges
        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Calculate camera's top, bottom, left, and right edges
        float topEdge = mainCamera.transform.position.y + cameraHeight;
        float bottomEdge = mainCamera.transform.position.y - cameraHeight;
        float leftEdge = mainCamera.transform.position.x - cameraWidth;
        float rightEdge = mainCamera.transform.position.x + cameraWidth;

        // Get the player's position
        Vector3 playerPosition = player.position;

        // Check if the player touches any of the camera's borders
        if (playerPosition.y >= topEdge || playerPosition.y <= bottomEdge ||
            playerPosition.x <= leftEdge || playerPosition.x >= rightEdge)
        {
            PlayerLost();  // Player loses when they touch the camera's edge
        }
    }

    void PlayerLost()
    {
        Debug.Log("Player Lost!");  // Handle the loss condition here
        // Implement game over logic, like showing Game Over UI or restarting the level
    }
}

