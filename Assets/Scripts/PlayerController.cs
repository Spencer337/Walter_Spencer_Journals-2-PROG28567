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
    public float acceleration;
    public float deceleration;
    public Vector2 horizontalVelocity, verticalVelocity;
    public bool isFacingLeft = true;
    public ContactPoint2D[] contacts;
    public int numOfContacts;
    public float apexHeight;
    public float apexTime;
    public float playerGravity, initialJumpVelocity;
    private bool jumpTriggered = false;
    private Vector2 playerInput = new Vector2();
    public float terminalSpeed = -5;
    public float coyoteTime, hangTime;
    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        acceleration = maxSpeed / accelerationTime;
        deceleration = maxSpeed / decelerationTime;
        contacts = new ContactPoint2D[10];
        playerGravity = -2 * apexHeight / (Mathf.Pow(apexTime, 2));
        initialJumpVelocity = 2 * apexHeight / apexTime;
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
        if (Input.GetKeyDown(KeyCode.Space) && hangTime < coyoteTime)
        {
            jumpTriggered = true;
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


        if (IsGrounded() == false)
        {
            verticalVelocity.y += playerGravity * Time.deltaTime;
            hangTime += Time.deltaTime;
        }
        else if (IsGrounded() == true && verticalVelocity.y < 0)
        {
            hangTime = 0;
            verticalVelocity.y = 0;
        }

        if (jumpTriggered == true)
        {
            verticalVelocity.y = 0;
            hangTime = coyoteTime;
            verticalVelocity.y += initialJumpVelocity;
            jumpTriggered = false;
        }

        // If the player is falling faster than the terminal speed, set vertical velocity to the terminal speed
        if (verticalVelocity.y < terminalSpeed)
        {
            verticalVelocity.y = terminalSpeed;
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
