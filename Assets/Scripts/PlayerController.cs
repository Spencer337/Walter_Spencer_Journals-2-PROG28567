using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D selfRigidBody;
    public float maxSpeed = 5;
    public float accelerationTime = 2;
    public float decelerationTime = 1;
    private float acceleration;
    private float deceleration;
    public Vector2 horizontalVelocity, verticalVelocity;
    public bool isFacingLeft = true;
    public ContactPoint2D[] contacts;
    public int numOfContacts;
    public float apexHeight;
    public float apexTime;
    public float playerGravity, initialJumpVelocity;
    public bool jumpTriggered, doubleJumpTriggered, jumpSpent, doubleJumpSpent;
    public float doubleApexHeight, doubleApexTime;
    public float doubleJumpGravity, doubleJumpVelocity;
    private Vector2 playerInput = new Vector2();
    public float terminalSpeed = -5;
    public float doubleJumpTerminalSpeed = -3;
    public float coyoteTime, hangTime;
    public float dashDistance, dashTime;
    public Vector2 startPosition, endPosition;
    public bool isDashing = false;
    public AnimationCurve dashCurve;
    public float dashCurveTime, maxDashSpeed;
    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        jumpTriggered = false;
        doubleJumpTriggered = false;
        jumpSpent = false;
        jumpSpent = false;
        doubleApexHeight = apexHeight * 0.75f;
        doubleApexTime = apexTime * 0.75f;
        maxDashSpeed = maxSpeed + dashDistance;
        acceleration = maxSpeed / accelerationTime;
        deceleration = maxSpeed / decelerationTime;
        contacts = new ContactPoint2D[10];
        playerGravity = -2 * apexHeight / (Mathf.Pow(apexTime, 2));
        initialJumpVelocity = 2 * apexHeight / apexTime;
        doubleJumpGravity = -1.5f * doubleApexHeight / (Mathf.Pow(doubleApexTime, 2));
        doubleJumpVelocity = 2 * doubleApexHeight / doubleApexTime;

    }

    // Update is called once per frame
    void Update()
    {
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        playerInput = Vector2.zero;
        if(Input.GetKey(KeyCode.A))
        {
            playerInput = Vector2.left;
            isFacingLeft = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            playerInput = Vector2.right;
            isFacingLeft = false;
        }
        //MovementUpdate(playerInput);
        if (Input.GetKeyDown(KeyCode.Space) && hangTime < coyoteTime && jumpSpent == false)
        {
            jumpTriggered = true;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && jumpSpent == true && doubleJumpSpent == false)
        {
            doubleJumpTriggered = true;
        }
        // If the player presses the left shift button, they dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && isDashing == false)
        {
            if (isFacingLeft == true)
            {
                endPosition.x = horizontalVelocity.x -dashDistance;
            }
            else
            {
                endPosition.x = horizontalVelocity.x + dashDistance;
            }
            startPosition.x = horizontalVelocity.x;
            isDashing = true;
        }
        MovementUpdate();
    }

    private void FixedUpdate()
    {
        selfRigidBody.linearVelocity = horizontalVelocity + verticalVelocity;
    }

    
    private void MovementUpdate()
    {
        if (playerInput.magnitude > 0)
        {
            horizontalVelocity += playerInput * acceleration * Time.deltaTime;
            if (horizontalVelocity.magnitude > maxSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            }
        }
        else
        {
            Vector2 changeInVelocity = horizontalVelocity.normalized * deceleration * Time.deltaTime;
            if (changeInVelocity.magnitude > horizontalVelocity.magnitude)
            {
                horizontalVelocity = Vector2.zero;
            }
            else
            {
                horizontalVelocity -= changeInVelocity;
            }

        }

        // Standard gravity
        if (IsGrounded() == false && isDashing == false)
        {
            verticalVelocity.y += playerGravity * Time.deltaTime;
            hangTime += Time.deltaTime;
            // If the player is falling faster than the terminal speed, set vertical velocity to the terminal speed
            if (verticalVelocity.y < terminalSpeed)
            {
                verticalVelocity.y = terminalSpeed;
            }
        }
        // Double jump gravity
        if (IsGrounded() == false && doubleJumpSpent == true)
        {
            verticalVelocity.y += doubleJumpGravity * Time.deltaTime;
            hangTime += Time.deltaTime;
            // If the player is falling faster than the terminal speed, set vertical velocity to the terminal speed
            if (verticalVelocity.y < doubleJumpTerminalSpeed)
            {
                verticalVelocity.y = doubleJumpTerminalSpeed;
            }
        }
        if (IsGrounded() == true && verticalVelocity.y < 0)
        {
            hangTime = 0;
            verticalVelocity.y = 0;
            jumpSpent = false;
            doubleJumpSpent = false;
        }

        if (jumpTriggered == true && isDashing == false)
        {
            verticalVelocity.y = 0;
            hangTime = coyoteTime;
            verticalVelocity.y += initialJumpVelocity;
            jumpTriggered = false;
            jumpSpent = true;
        }
        else if (jumpTriggered == true &&  isDashing == true)
        {
            jumpTriggered = false;
        }

        if (doubleJumpTriggered == true)
        {
            verticalVelocity.y = 0;
            horizontalVelocity.x = 0;
            hangTime = coyoteTime;
            verticalVelocity.y += doubleJumpVelocity;
            doubleJumpTriggered = false;
            doubleJumpSpent = true;
        }

        // Dash management
        if (isDashing == true)
        {
            verticalVelocity.y = 0;
            horizontalVelocity = Vector2.Lerp(startPosition, endPosition, dashTime);
            dashCurveTime += Time.deltaTime;
            dashTime = 1 * dashCurve.Evaluate(dashCurveTime);
            if (horizontalVelocity.magnitude > maxDashSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxDashSpeed;
            }
        }
        if (dashTime > 1 || dashCurveTime > 1)
        {
            dashTime = 0;
            dashCurveTime = 0;
            isDashing = false;
        }

    }

    public bool IsWalking()
    {
        if (horizontalVelocity.magnitude > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }
    public bool IsGrounded()
    {
        for (int i = 0; i < numOfContacts; i++)
        {
            if (contacts[i].normal.y == 1)
            {
                return true;
            }
        }
        return false;
        
    }

    public FacingDirection GetFacingDirection()
    {
        if (isFacingLeft == true)
        {
            return FacingDirection.left;
        }
        else 
        {
            return FacingDirection.right;
        }
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        numOfContacts = collision.GetContacts(contacts);
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        numOfContacts = collision.GetContacts(contacts);
    }
}
