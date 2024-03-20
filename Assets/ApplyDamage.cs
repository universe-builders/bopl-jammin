using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyDamage : MonoBehaviour
{
    public Collider2D collider;

    public float damage = 1.0f;

    private void Start()
    {
        collider = GetComponent<Collider2D>();
    }

    void Update()
    {
        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        collider.GetContacts(contacts);
        foreach(ContactPoint2D contact in contacts)
        {
            Damage damage = contact.rigidbody.GetComponent<Damage>();
            if(damage != null)
            {
                damage.health -= Time.deltaTime;
            }
        }
    }
}
