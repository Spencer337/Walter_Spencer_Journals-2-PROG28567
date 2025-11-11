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
    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        acceleration = maxSpeed / accelerationTime;
        deceleration = maxSpeed / decelerationTime;
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
        }
        else if (Input.GetKey(KeyCode.D))
        {
            playerInput = Vector2.right;
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
            velocity -= velocity.normalized * deceleration * Time.deltaTime;
        }
        selfRigidBody.linearVelocity = velocity;
    }

    public bool IsWalking()
    {
        return false;
    }
    public bool IsGrounded()
    {
        return true;
    }

    public FacingDirection GetFacingDirection()
    {
        return FacingDirection.left;
    }
}
