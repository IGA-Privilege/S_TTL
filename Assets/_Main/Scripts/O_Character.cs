using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_Character : Singleton<O_Character>
{
    public float moveSpeed;
    public float dashDistance;
    private Rigidbody2D rb_Character;

    // Start is called before the first frame update
    void Start()
    {
        rb_Character = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horiAxis = Input.GetAxis("Horizontal");
        float verAxis = Input.GetAxis("Vertical");

        Vector2 direction = new Vector2(horiAxis, verAxis).normalized;

        if (direction!=Vector2.zero)
        {
            rb_Character.velocity = direction * moveSpeed;
        }
        else
        {
            rb_Character.velocity = Vector2.zero;
        }

        if (horiAxis>0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if(horiAxis<0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
