using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
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

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetButtonDown("Jump");
        jumpReleased = Input.GetButtonUp("Jump");

        if (jumpInput && IsGrounded())
        {
            //Jump();
            rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
        }

        if (jumpReleased && rb.velocity.y > 0) {
            //Debug.Log("TRUE");
            rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
        }

        if (rb.velocity.y < 0) {
            rb.gravityScale = gravityScale * fallGravityMultiplier;
        }
        else {
            rb.gravityScale = gravityScale;
        }

        /*if (Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer) && isJumping) 
		{
            isJumping = false;
        }*/
        WallSlide();
        WallJump();

        if (!isWallJumping)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        //X axis Movement
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
}
