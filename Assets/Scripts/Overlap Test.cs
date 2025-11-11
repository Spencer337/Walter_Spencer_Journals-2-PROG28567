using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OverlapTest : MonoBehaviour
{
    public TextMeshProUGUI overlapText;
    public Collider2D selfCollider;
    public int numOfCollisions;
    public List<Collider2D> results;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        results = new List<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        results.Add(collision);

        numOfCollisions = Physics2D.OverlapCollider(selfCollider, results);
        overlapText.text = "Overlapping Colliders: " + numOfCollisions.ToString();
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        results.Remove(collision);

        numOfCollisions = Physics2D.OverlapCollider(selfCollider, results);
        overlapText.text = "Overlapping Colliders: " + numOfCollisions.ToString();
    }
}