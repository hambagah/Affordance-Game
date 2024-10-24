using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private KeyCode[] keys2 = {KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z, KeyCode.Space};
    public Animator animator;

    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;
    private TMP_Text leftText;
    private TMP_Text rightText;
    private TMP_Text jumpText;


    private float horizontal;
    private bool jumpInput;
    private bool jumpReleased;
    public float speed = 15f;
    public float jump = 25f;
    public float velPower = 1.1f;
    public float acceleration = 4f;
    public float decceleration = 4f;
    public float jumpCutMultiplier = 0.5f;
    public float gravityScale = 5f;
    public float fallGravityMultiplier = 1.4f;
    public float pushStrength = 2f;
    private bool stunned = false;
    public float stunTime = 0.75f;

    private bool isJumping = false;
    private bool jumpInputRelease = false;
    private bool isFacingRight = true;
    private bool wallSliding;
    public float wallSlidingSpeed = 0.9f;
    public float wallSlidingMultiplier = 1.4f;
    public float yvelo;

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

    
    public AudioClip hit;
    public AudioClip jumpSound;
    public AudioClip pickup;
    AudioSource audio;
    
    void Start()
    {
        leftText = GameObject.Find("LeftText").GetComponent<TMP_Text>();
        rightText = GameObject.Find("RightText").GetComponent<TMP_Text>();
        jumpText = GameObject.Find("JumpText").GetComponent<TMP_Text>();
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        horizontal = (Input.GetKey(leftKey) ? -1 : 0) + (Input.GetKey(rightKey) ? 1 : 0);

        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        if (IsGrounded() && !wallSliding)
            animator.SetBool("IsJumping", false);
        else 
            animator.SetBool("IsJumping", true);
            animator.SetFloat("Yspeed",  rb.velocity.y);


        yvelo = rb.velocity.y;

        jumpInput = Input.GetKeyDown(jumpKey);
        jumpReleased = Input.GetKeyUp(jumpKey);

        if (jumpInput && IsGrounded())
        {
            rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
            audio.clip = jumpSound;
            audio.Play();
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

        WallSlide();
        WallJump();

        if (!isWallJumping)
        {
            Flip();
        }
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
            animator.SetBool("Sliding", true);
            float speedDif = wallSlidingSpeed - rb.velocity.y;	
            float movement = speedDif * wallSlidingMultiplier;
            movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

            rb.AddForce(movement * Vector2.up);
        }
        else
        {
            wallSliding = false;
            animator.SetBool("Sliding", false);
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
            audio.clip = jumpSound;
            audio.Play();
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
        animator.SetBool("Stunned", false);
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
        speed = 15f;
        Retext();
        audio.clip = hit;
        audio.Play();

        isWallJumping = false;
        wallJumperCounter = wallJumpingTime;

        CancelInvoke(nameof(StopWallJumping));
        animator.SetBool("Stunned", true);
        
        wallJumperCounter -= Time.deltaTime;
        

        if (wallJumperCounter > 0f)
        {
            isWallJumping = true;
            Vector2 force = new Vector2(hitPower.x, hitPower.y);
            force.x *= direction;

            if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))
                force.x -= rb.velocity.x;

            rb.AddForce(force, ForceMode2D.Impulse);
            wallJumperCounter = 0f;
            Invoke(nameof(StopWallJumping), stunTime);
        }
    }

    public void RandomKey()
    {
        int random = Random.Range(0, 3);
        if (random == 0) {
            //leftKey = keys2[Random.Range(0, keys2.Length)];
            leftKey = GenRandomKey();
        }
        else if (random == 1) {
            //rightKey = keys2[Random.Range(0, keys2.Length)];
            rightKey = GenRandomKey();
        }
        else if (random == 2) {
            //jumpKey = keys2[Random.Range(0, keys2.Length)];
            jumpKey = GenRandomKey();
        }
        audio.clip = pickup;
        audio.Play();
        Retext();
    }

    public void RandomSlow()
    {
        speed = Random.Range(5, 12);
        audio.clip = pickup;
        audio.Play();
    }

    private KeyCode GenRandomKey()
    {
        KeyCode newKey;
        do
        {
            newKey = keys2[Random.Range(0, keys2.Length-1)];
        } while (newKey == leftKey || newKey == rightKey || newKey == jumpKey);
        return newKey;
    }

    private void Retext()
    {
        leftText.text = leftKey.ToString();
        rightText.text = rightKey.ToString();
        jumpText.text = jumpKey.ToString();
    }
}
