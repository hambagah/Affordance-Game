using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    private Movement player;
    public int type = 0;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Movement>();
    }

    void Update()
    {

        if (TouchPlayer())
        {
            if (type == 0)
                player.RandomKey();
                transform.Rotate (new Vector3 (0, Time.deltaTime * 45, 0));
            if (type == 1)
                player.RandomKey();
            Destroy(gameObject);
        }    
    }
    
    private bool TouchPlayer()
    {
        return Physics2D.OverlapCircle(transform.position, 0.2f, playerLayer);
    }
}
