using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class TransitionMovementStateIfInAir : MonoBehaviour
{
    MovementStateMachine movementStateMachine = null;
    CircleCollider2D collider = null;
    SplineGravity gravity;

    public float groundDistanceTolerance = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        movementStateMachine = GetComponent<MovementStateMachine>();
        collider = GetComponent<CircleCollider2D>();
        gravity = GetComponent<SplineGravity>();
    }

    // Update is called once per frame
    void Update()
    {
        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        collider.GetContacts(contacts);
        
        float distanceFromGround = gravity.DistanceToNearestGround();
        bool isInAir = contacts.Count == 0 && distanceFromGround > groundDistanceTolerance;

        if (isInAir && movementStateMachine.GetMovementState() != MovementState.AirMovement)
        {
            movementStateMachine.TransitionTo(MovementState.AirMovement);
        }
        else if(!isInAir && movementStateMachine.GetMovementState() == MovementState.AirMovement)
        {
            movementStateMachine.TransitionTo(MovementState.GroundMovement);
        }
    }
}
