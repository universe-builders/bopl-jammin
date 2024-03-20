using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionMovementStateIfHoldingSpace : MonoBehaviour
{
    MovementStateMachine movementStateMachine = null;
    Inputs inputs = null;

    // Start is called before the first frame update
    void Start()
    {
        movementStateMachine = GetComponent<MovementStateMachine>();
        inputs = GetComponent<Inputs>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movementStateMachine.GetMovementState() == MovementState.ChargingJumpMovement) return;

        int gamepadIndex = inputs.GamepadIndex();
        if (Input.GetButton($"Jump-GP-{gamepadIndex}")
            || Input.GetButtonDown($"Jump-GP-{gamepadIndex}")
            || Input.GetButton($"Jump-KB")
            || Input.GetButtonDown($"Jump-KB")
        )
        {
            movementStateMachine.TransitionTo(MovementState.ChargingJumpMovement);
        }
    }
}
