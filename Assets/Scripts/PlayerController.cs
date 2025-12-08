using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

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
    public float dashDistance, dashTime, minDashDistance;
    public Vector2 startPosition, endPosition;
    public bool isDashing = false;
    public AnimationCurve dashCurve;
    public float dashCurveTime;
    public bool gravityEnabled = true;
    public float reverseGravSpeed = 0.5f;
    public int health = 3;

    public CharacterState currentState = CharacterState.idle;
    public CharacterState previousState = CharacterState.idle;

    public enum FacingDirection
    {
        left, right
    }

    public enum CharacterState
    {
        idle, walk, jump, death, falling, doublejump, dash
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set the jump booleans to false
        jumpTriggered = false;
        doubleJumpTriggered = false;
        jumpSpent = false;
        jumpSpent = false;

        // Set the properties of the jump and double jump variables
        doubleApexHeight = apexHeight * 0.95f;
        doubleApexTime = apexTime * 0.75f;
        playerGravity = -2 * apexHeight / (Mathf.Pow(apexTime, 2));
        initialJumpVelocity = 2 * apexHeight / apexTime;
        doubleJumpGravity = -1.5f * doubleApexHeight / (Mathf.Pow(doubleApexTime, 2));
        doubleJumpVelocity = 2 * doubleApexHeight / doubleApexTime;

        // Set the properties of the horiztonal movement variables
        acceleration = maxSpeed / accelerationTime;
        deceleration = maxSpeed / decelerationTime;

        // Create a list of contact points to check for grounding
        contacts = new ContactPoint2D[10];
    }

    // Update is called once per frame
    void Update()
    {
        // Reset the playerInput vector
        playerInput = Vector2.zero;
        // If the player presses the A key, set player input to left and have them face left
        if(Input.GetKey(KeyCode.A))
        {
            playerInput = Vector2.left;
            isFacingLeft = true;
        }
        // If the player presses the D key, set player input to right and have them face right
        else if (Input.GetKey(KeyCode.D))
        {
            playerInput = Vector2.right;
            isFacingLeft = false;
        }
        // If the player presses the space key and they can jump, trigger a jump
        if (Input.GetKeyDown(KeyCode.Space) && hangTime < coyoteTime && jumpSpent == false)
        {
            jumpTriggered = true;
        }
        // If the player presses the space key can they already jumped, trigger a double jump
        else if (Input.GetKeyDown(KeyCode.Space) && jumpSpent == true && doubleJumpSpent == false)
        {
            doubleJumpTriggered = true;
        }
        // If the player presses the left shift button, trigger a dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && isDashing == false)
        {
            // If the player is facing left, set the dash end positon to the left of the player
            if (isFacingLeft == true)
            {
                endPosition.x = horizontalVelocity.x - dashDistance;
                // If the end position falls short of the minimum dash distance, set the end position to the dash distance
                if (endPosition.x > -minDashDistance)
                {
                    endPosition.x = -minDashDistance;
                }
            }
            // If the player is facing right, set the dash end positon to the right of the player
            else
            {
                endPosition.x = horizontalVelocity.x + dashDistance;
                // If the end position falls short of the minimum dash distance, set the end position to the dash distance
                if (endPosition.x < minDashDistance)
                {
                    endPosition.x = minDashDistance;
                }
            }
            startPosition.x = horizontalVelocity.x;
            isDashing = true;
        }

        // Update the player's movement and animation state
        MovementUpdate();
        StateUpdate();
    }

    private void FixedUpdate()
    {
        // Set the player's rigidbody velocity to equal the horizontal and vertical velocities
        selfRigidBody.linearVelocity = horizontalVelocity + verticalVelocity;
        // Set the player's rotation. If gravity is not enabled and the player is not dashing, rotate them 180 degrees
        if (gravityEnabled == false && isDashing == false)
        {
            selfRigidBody.MoveRotation(180);
        }
        // Otherwise, set their rotation back to zero
        else
        {
            selfRigidBody.MoveRotation(0);
        }
    }

    
    private void MovementUpdate()
    {
        // If the player is pressing a move key, move the player in that direction
        if (playerInput.magnitude > 0)
        {
            horizontalVelocity += playerInput * acceleration * Time.deltaTime;
            
            // If the player is moving greater than max speed, set them back to max speed
            if (horizontalVelocity.magnitude > maxSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            }
        }
        // If the player is not pressing a move key, begin decelerating
        else
        {
            Vector2 changeInVelocity = horizontalVelocity.normalized * deceleration * Time.deltaTime;
            // If the player is going to overshoot their deceleration, set horizontal velocity to zero
            if (changeInVelocity.magnitude > horizontalVelocity.magnitude)
            {
                horizontalVelocity = Vector2.zero;
            }
            // Otherwise, decelerate normally
            else
            {
                horizontalVelocity -= changeInVelocity;
            }
        }

        // If the player is dashing, have their horizontal velocity linearlly interpolate between it's current positon, and that position increased by dash distance
        if (isDashing == true)
        {
            verticalVelocity.y = 0;
            horizontalVelocity = Vector2.Lerp(startPosition, endPosition, dashTime);
            dashCurveTime += Time.deltaTime;
            dashTime = 1 * dashCurve.Evaluate(dashCurveTime);
        }
        // If the dashTime or dashCurveTime are greater than 1, set them back to zero and end the dash
        // If the player presses the dash key again, also end the dash
        if (dashCurveTime > 1 || Input.GetKeyDown(KeyCode.Space))
        {
            dashTime = 0;
            dashCurveTime = 0;
            isDashing = false;
        }

        // If the player is in standard gravity, increase their vertical velocity by the playerGravity variable
        if (IsGrounded() == false && isDashing == false && gravityEnabled == true)
        {
            verticalVelocity.y += playerGravity * Time.deltaTime;
            hangTime += Time.deltaTime;
            // If the player is falling faster than the terminal speed, set vertical velocity to the terminal speed
            if (verticalVelocity.y < terminalSpeed)
            {
                verticalVelocity.y = terminalSpeed;
            }
        }
        // If the player is in double jump gravity, increase their vertical velocity by the doubleJumpGravity variable
        if (IsGrounded() == false && doubleJumpSpent == true && gravityEnabled == true)
        {
            verticalVelocity.y += doubleJumpGravity * Time.deltaTime;
            // If the player is falling faster than the double jump terminal speed, set vertical velocity to the double jump terminal speed
            if (verticalVelocity.y < doubleJumpTerminalSpeed)
            {
                verticalVelocity.y = doubleJumpTerminalSpeed;
            }
        }
        // If the player is grounded and not in an anti gravity zone, reset jump related variables
        if (IsGrounded() == true && verticalVelocity.y < 0 && gravityEnabled == true)
        {
            hangTime = 0;
            verticalVelocity.y = 0;
            jumpSpent = false;
            doubleJumpSpent = false;
        }

        // If a jump is input, the character jumps
        if (jumpTriggered == true)
        {
            verticalVelocity.y = 0;
            hangTime = coyoteTime;
            verticalVelocity.y += initialJumpVelocity;
            jumpTriggered = false;
            jumpSpent = true;
        }
        // If a double jump is triggered, the character performs the double jump
        if (doubleJumpTriggered == true)
        {
            verticalVelocity.y = 0;
            horizontalVelocity.x = 0;
            verticalVelocity.y += doubleJumpVelocity;
            doubleJumpTriggered = false;
            doubleJumpSpent = true;
        }
        
    }

    private void StateUpdate()
    {
        // Update the previous state to the current state
        previousState = currentState;

        // If the player is walking and on the ground, set them to the walk state
        if (IsWalking() && IsGrounded())
        {
            currentState = CharacterState.walk;
        }
        // If the player has double jumped and is moving up, set them to the double jump state
        else if (doubleJumpSpent == true && verticalVelocity.y > 0.5)
        {
            currentState = CharacterState.doublejump;
        }
        // If the player is in a reverse gravity zone, set them to the falling state
        else if (gravityEnabled == false)
        {
            currentState = CharacterState.falling;
        }
        // If the player has jumped and is moving up, set them to the jump state
        else if (!IsGrounded() && verticalVelocity.y > 0.5)
        {
            currentState = CharacterState.jump;
        }
        // If the player is in the air and is moving down, set them to the falling state
        else if (!IsGrounded() && verticalVelocity.y <= 0.5)
        {
            currentState = CharacterState.falling;
        }
        // Otherwise, set them to the idle state
        else
        {
            currentState = CharacterState.idle;
        }
        //,If the has died, set them to the death state
        if(HasDied() == true)
        {
            currentState = CharacterState.death;
        }
        // If the player is dashing, set them to the dash state
        if(isDashing == true)
        {
            currentState = CharacterState.dash;
        }
    }

    // If the player's health is 0 or less return true. Otherwise, return false
    public bool HasDied()
    {
        if (health <= 0)
        {
            return true;
        }
        return false;
    }

    // If the player's horizontal magnitude is more than 0 return true. Otherwise, return false
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

    // If any of the player's contact points are on the ground return true. Otherwise, return false
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

    // Set the value of the FacingDirection enum based on the isFacingLeft variable
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

    // While the player is colliding with an object, get the number of contact points
    public void OnCollisionStay2D(Collision2D collision)
    {
        numOfContacts = collision.GetContacts(contacts);
    }

    // When the player stops colliding with an object, get the number of contact points
    public void OnCollisionExit2D(Collision2D collision)
    {
        numOfContacts = collision.GetContacts(contacts);
    }

    // When the player enters a reverse gravity zone and is not dashing, rotate them upside down and set gravityEnabled to false
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            gravityEnabled = false;
            verticalVelocity.y = 0;
        }
    }

    // If the player is in a reverse gravity zone, increase their vertical velocity
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            verticalVelocity.y -= playerGravity * Time.deltaTime * reverseGravSpeed;
        }
    }

    // When the player exits a reverse gravity zone, rotate them rightside up, and set gravity back to normal
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            gravityEnabled = true;
        }
    }

    // Once the dying animation is complete, disable the character
    public void OnDyingAnimationComplete()
    {
        gameObject.SetActive(false);
    }
}
