using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(Animator))]

public class SoyBoyController : MonoBehaviour
{
    public float speed = 14f;
    public float accel = 6f;
    private Vector2 input;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator animator;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        // Get X and Y values from built-in Unity control axes named Horizontal and Jump
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Jump");
        
        // If greater than 0, player is facing right
        if(input.x > 0f)
        {
            sr.flipX = false;
        }
        else if (input.x < 0f)
        {
            sr.flipX = true;
        }
    }

    void FixedUpdate()
    {
        // Assign value of accel to private variable named acceleration
        var acceleration = accel;
        var xVelocity = 0f;
        
        // If horizontal axis controls are neutral, then xVelocity is set to 0
        if (input.x == 0)
        {
            xVelocity = 0f;
        }
        else
        {
            xVelocity = rb.velocity.x;
        }

        // Force is added to rb by calculating current value of horizontal axis controls multiplied by speed, which is in turn multiplied by acceleration
        rb.AddForce(new Vector2(((input.x * speed) - rb.velocity.x) * acceleration, 0));
        // Velocity is reset on rb so it can stop Super Soy Boy from moving left or right when controls are in neutral state
        rb.velocity = new Vector2(xVelocity, rb.velocity.y);
    }
}
