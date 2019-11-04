﻿using System.Collections;
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

    public bool isJumping;
    public float jumpSpeed = 8f;
    private float rayCastLengthCheck = 0.005f;
    private float width;
    private float height;

    public float jumpDurationThreshold = 0.25f;
    private float jumpDuration;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        width = GetComponent<Collider2D>().bounds.extents.x + 0.1f;
        height = GetComponent<Collider2D>().bounds.extents.y + 0.2f;
    }

    void Start()
    {
        
    }

    public bool PlayerIsOnGround()
    {
        //perform Raycast directly below character
        bool groundCheck1 = Physics2D.Raycast(new Vector2(
            transform.position.x, transform.position.y - height),
            -Vector2.up, rayCastLengthCheck);
        bool groundCheck2 = Physics2D.Raycast(new Vector2(
            transform.position.x + (width - 0.2f), transform.position.y - height),
            -Vector2.up, rayCastLengthCheck);
        bool groundCheck3 = Physics2D.Raycast(new Vector2(
            transform.position.x - (width - 0.2f), transform.position.y - height),
            -Vector2.up, rayCastLengthCheck);

        //if any ground check is true, returns true
        if (groundCheck1 || groundCheck2 || groundCheck3)
        {
            return true;
        }
        else
        {
            return false;
        }
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

        if (input.y >= 1f)
        {
            jumpDuration += Time.deltaTime;
        }
        else
        {
            isJumping = false;
            jumpDuration = 0f;
        }

        if (PlayerIsOnGround() && isJumping == false)
        {
            if (input.y > 0f)
            {
                isJumping = true;
            }
        }

        if (jumpDuration > jumpDurationThreshold) input.y = 0f;
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

        if (isJumping && jumpDuration < jumpDurationThreshold)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }
}