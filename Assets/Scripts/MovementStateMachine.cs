using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementState
{
    On_Ground,
    In_Air
}

public class MovementStateMachine : MonoBehaviour
{
    MovementState state = MovementState.On_Ground;

    SplineMovement splineMovement;
    AirMovement airMovement;
    SplineGravity splineGravity;

    public MovementState GetMovementState => state;
    public void TransitionTo(MovementState transitionTo)
    {
        Debug.Assert(transitionTo != state);

        switch (state)
        {
            case MovementState.On_Ground:
                splineMovement.enabled = false;
                splineGravity.enabled = false;
                break;
            case MovementState.In_Air:
                airMovement.enabled = false;
                break;
        }

        state = transitionTo;

        switch (state)
        {
            case MovementState.On_Ground:
                splineMovement.enabled = true;
                splineGravity.enabled = true;
                break;
            case MovementState.In_Air:
                airMovement.enabled = true;
                break;
        }
    }

    private void Start()
    {
        splineMovement = GetComponent<SplineMovement>();
        airMovement = GetComponent<AirMovement>();
        splineGravity = GetComponent<SplineGravity>();
    }
}
