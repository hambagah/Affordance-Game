using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float pSpeed;
    public int startLocation;
    public Transform[] points;
    private int i;
    public bool grippable;

    void Start()
    {
        transform.position = points[startLocation].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, points[i].position) < 0.05f)
        {
            i++;
            Debug.Log("HELLO");
            if (i == points.Length)
            {
                i = 0;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, points[i].position, pSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (grippable)
            col.transform.SetParent(transform);
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (grippable)
            col.transform.SetParent(null);
    }
}
