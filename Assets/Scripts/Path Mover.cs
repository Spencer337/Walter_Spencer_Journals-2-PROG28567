using Unity.VisualScripting;
using UnityEngine;

public class PathMover : MonoBehaviour
{
    public Vector3 startPosition, endPostion;
    public Transform startHolder, endHolder;

    public float duration;
    private float timeMoving = 0f;

    public enum State
    {
        forward, backward
    }
    public State currentState = State.forward;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeMoving += Time.deltaTime;
        if (currentState == State.forward)
        {
            transform.position = Vector3.Lerp(startHolder.position, endHolder.position, timeMoving / duration);
        }
        else if (currentState == State.backward)
        {
            transform.position = Vector3.Lerp(endHolder.position, startHolder.position, timeMoving / duration);
        }

        if (timeMoving >= duration)
        {
            if (currentState == State.forward)
            {
                currentState = State.backward;
            }
            else if (currentState == State.backward)
            {
                currentState = State.forward;
            }
            timeMoving = 0f;
        }
    }
}

