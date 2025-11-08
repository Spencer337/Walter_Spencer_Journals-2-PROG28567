using UnityEngine;

public class CannonballController : MonoBehaviour
{
    public float lifespan;

    // Start is called before the first frame update
    void Start()
    {
        //After lifespan seconds, this object will be destroyed
        Destroy(gameObject, lifespan);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CannonballController cannonball = collision.gameObject.GetComponent<CannonballController>();
        if (cannonball != null)
        {
            Destroy(gameObject);
        }


    }
}