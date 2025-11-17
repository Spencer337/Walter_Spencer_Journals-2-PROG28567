using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D selfRigidBody;
    public float maxSpeed = 5;
    public float accelerationTime = 2;
    public float decelerationTime = 1;
    public float acceleration;
    public float deceleration;
    public Vector2 velocity;
    public bool isFacingLeft = true;
    public ContactPoint2D[] contacts;
    public int numOfContacts;
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
    }

    // Update is called once per frame
    void Update()
    {
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        Vector2 playerInput = new Vector2();
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
            MovementUpdate(playerInput);
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        if (playerInput.magnitude > 0)
        {
            velocity += playerInput * acceleration * Time.deltaTime;
            if (velocity.magnitude > maxSpeed)
            {
                velocity = velocity.normalized * maxSpeed;
            }
        }
        else
        {
            Vector2 changeInVelocity = velocity.normalized * deceleration * Time.deltaTime;
            if (changeInVelocity.magnitude > velocity.magnitude)
            {
                velocity = Vector2.zero;
            }
            else
            {
                velocity -= changeInVelocity;
            }

        }
        selfRigidBody.linearVelocity = velocity;
    }

    public bool IsWalking()
    {
        if (velocity.magnitude > 0)
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
