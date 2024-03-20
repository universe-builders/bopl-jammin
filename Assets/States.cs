using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class States : MonoBehaviour
{
    [SerializeField]
    bool isInAir = false;
    [SerializeField]
    bool isDescending = false;

    SplineGravity gravity;
    CircleCollider2D collider;
    Rigidbody2D body;

    [Tooltip("How far from the ground to be considered on it.")]
    public float groundDistanceTolerance = 0.2f;

    private void Start()
    {
        gravity = GetComponent<SplineGravity>();
        collider = GetComponent<CircleCollider2D>();
        body = GetComponent<Rigidbody2D>();
    }

    public bool IsDescending()
    {
        return isDescending;
    }

    public bool IsInAir()
    {
        return isInAir;
    }

    public bool IsOnGround()
    {
        return !isInAir;
    }

    private void Update()
    {
        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        collider.GetContacts(contacts);
        float distanceFromGround = gravity.DistanceToNearestGround();
        isInAir = contacts.Count == 0 && distanceFromGround > groundDistanceTolerance;

        if (isInAir)
        {
            if(body.velocityY < 0)
            {
                isDescending = true;
            }
        }
    }
}
