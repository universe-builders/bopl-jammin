using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementState
{
    GroundMovement,
    ChargingJumpMovement,
    SlamMovement,
    AirMovement
}

public class MovementStateMachine : MonoBehaviour
{
    [SerializeField]
    MovementState state = MovementState.GroundMovement;

    Rigidbody2D body;
    States states;
    SplineGravity splineGravity;
    Inputs inputs;
    
    SplineMovement groundMovement;
    ChargingJumpMovement chargingJumpMovement;
    SlamMovement slamMovement;
    AirMovement airMovement;

    public bool enableSlamming = false;

    public MovementState GetMovementState()
    {
        return state;
    }
    public void TransitionTo(MovementState transitionTo)
    {
        Debug.Assert(transitionTo != state);

        switch (state)
        {
            case MovementState.GroundMovement:
                splineGravity.enabled = false;
                groundMovement.enabled = false;
                break;
            case MovementState.ChargingJumpMovement:
                splineGravity.enabled = false;
                chargingJumpMovement.enabled = false;
                break;
            case MovementState.SlamMovement:
                slamMovement.enabled = false;
                break;
            case MovementState.AirMovement:
                airMovement.enabled = false;
                break;
        }

        state = transitionTo;

        switch (state)
        {
            case MovementState.GroundMovement:
                splineGravity.enabled = true;
                groundMovement.enabled = true;
                break;
            case MovementState.ChargingJumpMovement:
                splineGravity.enabled = true;
                chargingJumpMovement.enabled = true;
                break;
            case MovementState.SlamMovement:
                slamMovement.enabled = true;
                break;
            case MovementState.AirMovement:
                airMovement.enabled = true;
                break;
        }
    }

    private void AttemptTransitioning()
    {
        bool isOnGround = states.IsOnGround();
        bool isInAir = states.IsInAir();
        int gamepadIndex = inputs.GamepadIndex();
        bool isHoldingJumpButton = Input.GetButton($"Jump-GP-{gamepadIndex}") || Input.GetButtonDown($"Jump-GP-{gamepadIndex}") || Input.GetButton($"Jump-KB") || Input.GetButtonDown($"Jump-KB");

        if(isHoldingJumpButton && isOnGround && state != MovementState.ChargingJumpMovement)
        {
            TransitionTo(MovementState.ChargingJumpMovement);
        }
    }

    private void Start()
    {
        splineGravity = GetComponent<SplineGravity>();
        states = GetComponent<States>();
        inputs = GetComponent<Inputs>();

        airMovement = GetComponent<AirMovement>();
        slamMovement = GetComponent<SlamMovement>();
        groundMovement = GetComponent<SplineMovement>();
        chargingJumpMovement = GetComponent<ChargingJumpMovement>();
        body = GetComponent<Rigidbody2D>();
    }
    private void LateUpdate()
    {
        AttemptTransitioning();
    }
}
