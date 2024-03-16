using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class TransitionMovementStateIfInAir : MonoBehaviour
{
    MovementStateMachine movementStateMachine = null;
    CircleCollider2D collider = null;
    SplineProjector projector;
    SplineGravity gravity;

    public float groundDistanceTolerance = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        movementStateMachine = GetComponent<MovementStateMachine>();
        collider = GetComponent<CircleCollider2D>();
        projector = GetComponent<SplineProjector>();
        gravity = GetComponent<SplineGravity>();
    }

    // Update is called once per frame
    void Update()
    {
        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        collider.GetContacts(contacts);

        
        float distanceFromGround = gravity.DistanceToNearestGround();
        bool isInAir = contacts.Count == 0 && distanceFromGround > groundDistanceTolerance;

        if (isInAir && movementStateMachine.GetMovementState == MovementState.On_Ground)
        {
            movementStateMachine.TransitionTo(MovementState.In_Air);
        }
        else if(!isInAir && movementStateMachine.GetMovementState == MovementState.In_Air)
        {
            movementStateMachine.TransitionTo(MovementState.On_Ground);
        }
    }
}
