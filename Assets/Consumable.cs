using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    private Movement player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Movement>();
    }

    void Update()
    {
        if (TouchPlayer())
        {
            player.RandomKey();
            Destroy(gameObject);
        }    
    }
    
    private bool TouchPlayer()
    {
        return Physics2D.OverlapCircle(transform.position, 0.2f, playerLayer);
    }
}
