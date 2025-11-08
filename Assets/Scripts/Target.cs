using UnityEngine;

public class Target : MonoBehaviour
{
    public ScoreboardController scoreController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CannonballController cannonball = collision.gameObject.GetComponent<CannonballController>();
        if (cannonball != null)
        {
            scoreController.Score += 100;
        }

    }
}