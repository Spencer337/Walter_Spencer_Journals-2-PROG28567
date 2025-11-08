using UnityEngine;

public class RaycastTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 startPosition = Vector2.zero;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToFire = mousePosition;

        bool hitSomething = Physics2D.Raycast(startPosition, directionToFire);
        Color drawColour = Color.red;
        if (hitSomething)
        {
            drawColour = Color.green;
        }
        Debug.DrawLine(startPosition, directionToFire, drawColour);



    }
}