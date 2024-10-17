using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicKeybinds : MonoBehaviour
{
    public float moveSpeed = 5f;

    // Define default key bindings
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get Rigidbody2D component
    }

    void Update()
    {
        // Dynamic Keystroke Detection
        movement.x = (Input.GetKey(leftKey) ? -1 : 0) + (Input.GetKey(rightKey) ? 1 : 0);
        movement.y = (Input.GetKey(downKey) ? -1 : 0) + (Input.GetKey(upKey) ? 1 : 0);

        // Detecting Jump Buttons
        if (Input.GetKeyDown(jumpKey))
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        // Character movement
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    // jumping logic
    void Jump()
    {
        rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
    }

    // Allow external calls to modify key bindings
    public void ChangeKeybinds(KeyCode newUp, KeyCode newDown, KeyCode newLeft, KeyCode newRight, KeyCode newJump)
    {
        upKey = newUp;
        downKey = newDown;
        leftKey = newLeft;
        rightKey = newRight;
        jumpKey = newJump;
    }
}
