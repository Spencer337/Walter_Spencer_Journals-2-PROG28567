using UnityEngine;

public class Plane : MonoBehaviour
{
    public Rigidbody2D planeRigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Apply a force 
        planeRigidbody.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        //planeRigidbody.AddForce(-Vector3.up, ForceMode2D.Force);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("This object has just collided with another.");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("This object is currently touching another.");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("This object has stopped colliding with another.");
    }

}