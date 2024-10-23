using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float eSpeed;
    public bool startRight;
    public float pastSpeed;
    private float moveDelay;
    private bool touched = false;
    private float attackDelay = 0.4f;
    private Rigidbody2D eRB;
    [SerializeField] private Transform eGroundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform ePlayerCheck;
    [SerializeField] private LayerMask playerLayer;

    private Movement player;
    public Animator animator;


    void Start()
    {
        eRB = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").GetComponent<Movement>();
        pastSpeed = eSpeed;
        if (!startRight)
            EFlip();
    }

    void Update()
    {
        eRB.velocity = transform.right * eSpeed;
        animator.SetFloat("Speed", Mathf.Abs(eRB.velocity.x));
        if (!EIsGrounded())
            EFlip();

        if (TouchPlayer() )
        {
            if (attackDelay > 0.1f && touched)
            {
                eSpeed = 0f;
                touched = false;
                Attack();
                attackDelay = 0f;
            }
            else {
                attackDelay += Time.deltaTime;
                animator.SetBool("Attack", false);
            }
        }
        else if (moveDelay > 1f){
            eSpeed = pastSpeed;
            moveDelay = 0;
        }
        else 
            moveDelay += Time.deltaTime;
    }

    private bool EIsGrounded()
    {
        return Physics2D.OverlapCircle(eGroundCheck.position, 0.2f, groundLayer);
    }

    private bool TouchPlayer()
    {
        touched = true;
        return Physics2D.OverlapCircle(ePlayerCheck.position, 0.2f, playerLayer);
    }

    private void Attack()
    {
        float direction = 0f;
        if (transform.localScale.x > 0f) {
            direction = 1f;
        }
        else 
            direction = -1f;
        animator.SetBool("Attack", true);
        player.Hit(direction);
    }

    private void EFlip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
        eSpeed *= -1f;
        pastSpeed = eSpeed;
    }
}
