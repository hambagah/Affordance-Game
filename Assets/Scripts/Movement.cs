using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private string[] keys = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "SPACE"}; 
    private KeyCode[] keys2 = {KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G};
    
    private KeyCode leftKey = KeyCode.A;
    private KeyCode rightKey = KeyCode.D;
    private KeyCode jumpKey = KeyCode.Space;

    private KeyCode prevLeftKey = KeyCode.A;
    private KeyCode prevRightKey = KeyCode.D;


    private float horizontal;
    private bool jumpInput;
    private bool jumpReleased;
    public float speed = 15f;
    public float jump = 25f;
    public float velPower = 1.1f;
    public float acceleration = 15f;
    public float decceleration = 4f;
    public float jumpCutMultiplier = 0.5f;
    public float gravityScale = 5f;
    public float fallGravityMultiplier = 1.4f;
    public float pushStrength = 2f;
    private bool stunned = false;
    private float stunTime = 0f;
    /*public float frictionAmount = 1f;
    public float maxGravity = 1f;*/

    private bool isJumping = false;
    private bool jumpInputRelease = false;
    private bool isFacingRight = true;
    private bool wallSliding;
    public float wallSlidingSpeed = 0.9f;
    public float wallSlidingMultiplier = 1.4f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    public float wallJumpingTime = 0.2f;
    private float wallJumperCounter;
    public float wallJumpingDuration = 0.2f;
    public Vector2 wallJumpingPower = new Vector2(12f, 16f);

    public Vector2 hitPower = new Vector2(25f, 10f);
    public float hitMultiplier = 1.5f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    
    void Update()
    {
        horizontal = (Input.GetKey(leftKey) ? -1 : 0) + (Input.GetKey(rightKey) ? 1 : 0);
        jumpInput = Input.GetKeyDown(jumpKey);
        jumpReleased = Input.GetKeyUp(jumpKey);

        if (jumpInput && IsGrounded() && !stunned)
        {
            rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
        }

        if (jumpReleased && rb.velocity.y > 0) {
            rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
        }

        if (rb.velocity.y < 0) {
            rb.gravityScale = gravityScale * fallGravityMultiplier;
        }
        else {
            rb.gravityScale = gravityScale;
        }

        //Debug.Log(stunTime + " " + Time.deltaTime);

        if (stunned) 
            stunTime += Time.deltaTime;

        if (stunTime > 0.1f)
            stunTime = 0f;
            stunned = false;

        WallSlide();
        WallJump();

        if (!isWallJumping && !stunned)
        {
            Flip();
        }
        
        Debug.Log(leftKey + " " + rightKey + " " + jumpKey);
    }

    private void FixedUpdate()
    {
        float targetSpeed = horizontal * speed;
        float speedDiff = targetSpeed - rb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);

        if (!isWallJumping) {
            rb.AddForce(movement *  Vector2.right);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            wallSliding = true;
            float speedDif = wallSlidingSpeed - rb.velocity.y;	
            float movement = speedDif * wallSlidingMultiplier;
            movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

            rb.AddForce(movement * Vector2.up);
        }
        else
        {
            wallSliding = false;
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
        isJumping = true;
        jumpInputRelease = false;

    }

    private void WallJump()
    {   
        
        if (wallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumperCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumperCounter -= Time.deltaTime;
        }

        if (jumpInput && wallJumperCounter > 0f)
        {
            isWallJumping = true;
            Vector2 force = new Vector2(wallJumpingPower.x, wallJumpingPower.y);
            force.x *= wallJumpingDirection;

            if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))
                force.x -= rb.velocity.x;

            rb.AddForce(force, ForceMode2D.Impulse);
            wallJumperCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }
    
    private void Stunned()
    {
        isWallJumping = true;
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    public void Hit(float direction)
    {
        leftKey = KeyCode.A;
        rightKey = KeyCode.D;
        jumpKey = KeyCode.Space;

        Vector2 force = new Vector2(hitPower.x, hitPower.y);
        force.x *= direction;

        if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))
            force.x -= rb.velocity.x;
        
        rb.AddForce(force, ForceMode2D.Impulse);
        stunned = true;
    }

    public void RandomKey()
    {
        int random = Random.Range(0, 3);
        if (random == 0) {
            leftKey = keys2[Random.Range(0, keys2.Length)];
        }
        else if (random == 1) {
            rightKey = keys2[Random.Range(0, keys2.Length)];
        }
        else if (random == 2) {
            jumpKey = keys2[Random.Range(0, keys2.Length)];
        }
        Debug.Log(leftKey + " " + rightKey + " " + jumpKey);
    }
}
