using UnityEngine;

public class InterpolationTest : MonoBehaviour
{
    public Rigidbody2D selfRigidBody;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selfRigidBody.AddForce(Vector2.right * 5f, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selfRigidBody.AddForce(Vector2.right * 5f, ForceMode2D.Impulse);
        }
    }
}
