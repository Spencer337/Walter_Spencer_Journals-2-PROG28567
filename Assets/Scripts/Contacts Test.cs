using TMPro;
using UnityEngine;

public class ContactsTest : MonoBehaviour
{
    public TextMeshProUGUI contactsText;
    public Rigidbody2D selfRigidBody;
    public int numOfContacts;

    //public ContactPoint2D contacts;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        ContactPoint2D[] contacts = new ContactPoint2D[10];

        numOfContacts = collision.GetContacts(contacts);
        contactsText.text = "Contact Points: " + numOfContacts.ToString();
    }

    //public void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log("Colliding");
    //    ContactPoint2D[] contacts = new ContactPoint2D[10];

    //    numOfContacts = collision.GetContacts(contacts);
    //    contactsText.text = "Contact Points: " + numOfContacts.ToString();
    //}

    public void OnCollisionExit2D(Collision2D collision)
    {
        numOfContacts = 0;
        contactsText.text = "Contact Points: " + numOfContacts.ToString();
    }
}
