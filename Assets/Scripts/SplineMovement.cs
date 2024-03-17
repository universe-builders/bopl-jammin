using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public enum MovementDirection
{
    None,
    Forwards,
    Backwards,
};

public class SplineMovement : MonoBehaviour
{
    public bool debug = false;
    public Color debugForwardColor;
    public Color debugBackwardColor;
    public Color debugInputColor;
    public Color debugJumpDirectionColor;

    SplineGravity gravity;
    Rigidbody2D rb;
    Inputs inputs;

    [Tooltip("This curve is used to dictate the acceleration of the movement of the player.")]
    public AnimationCurve movementAccelerationCurve;

    [Tooltip("This is applied to the movement force each frame.")]
    public float movementSpeed = 300f;

    [Tooltip("This is applied to the jump impulse.")]
    public float jumpBaseImpulseMagnitude = 400.0f;

    [Tooltip("How long the space button has been held down, whilst on the ground. Used to add an extra force to the jump.")]
    public float spaceHeldDownTime = 0.0f;

    [Tooltip("The clamp for for spaceHeldDownTime when applying to the jump force.")]
    public float spaceHeldJumpImpulseMaximum = 1.0f;

    [Tooltip("How long the player has been moving for without stopping. Used to calculate the movement acceleration.")]
    public float moveInSameDirectionTime = 0.0f;

    [Tooltip("[0->1] How much to bias towards the previous direction, this means the movement is more likely to use the previous direction than a new direction which helps with sharp corners and provides time to allow the player to react.")]
    public float biasTowardsPreviousDirection = 0.6f;

    [Tooltip("The previous movement direction of the player. Used to bias towards same direction or new direction.")]
    public MovementDirection previousMovementDirection = MovementDirection.None;

    [Tooltip("[0 -> 1] How close to the forward/backward direction before starting to move. This allows the player holding down the perpindicular direction to forward without moving.")]
    public float overcomeBiasBeforeMoving = 0.3f;

    [Tooltip("[0 -> 1] The jump impulse consists of two forces, the normal of the surface and the direction of the input. At 0.0 the direction of input has no influence, at 1.0 the normal of the surface has no influence.")]
    public float ratioOfInputDirectionJumpImpulseForce = 0.5f;

    [Tooltip("If true, when player is not pressing any input then the full basis force will apply based on surface normal only.")]
    public bool ignoreInputDirectionJumpForceIfNoInput = true;

    [Tooltip("If true, when the player's input is directed towards inside the surface then ignore the input direction in the jump force calculation.")]
    public bool ignoreJumpInputDirectionIfInputIsDownwards = true;

    [Tooltip("[1.0 -> -1.0] Used for ignoring input direction in jump impulse if input is facing surface. At 0.0 then perpindicular angles are considered down, at -1.0 then only the vector exactly down is considered down. Between 0.0 and -1.0 will progressively get more 'down', whereas 0.0 to 1.0 is progressively more 'up'.")]
    public float isDownwardsTolerance = -0.6f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravity = GetComponent<SplineGravity>();
        inputs = GetComponent<Inputs>();
    }

    private void OnEnable()
    {
        // Reset variables as was not on the ground before this frame.
        spaceHeldDownTime = 0.0f;
        previousMovementDirection = MovementDirection.None;
    }

    void Update()
    {
        // Don't use gravity. Gravity is a real downer.
        rb.gravityScale = 0.0f;

        // Get input.
        int gamepadIndex = inputs.GamepadIndex();
        float hz = Input.GetAxis($"Horizontal-GP-{gamepadIndex}") + Input.GetAxis($"Horizontal");
        float vt = Input.GetAxis($"Vertical-GP-{gamepadIndex}") + Input.GetAxis($"Vertical");
        Vector2 inputDirection = new Vector2(hz, vt);
        float inputMagnitude = inputDirection.magnitude;
        inputDirection.Normalize();

        #region Jumping
        if (Input.GetButton($"Jump-GP-{gamepadIndex}") 
            || Input.GetButtonDown($"Jump-GP-{gamepadIndex}")
            || Input.GetButton($"Jump-KB")
            || Input.GetButtonDown($"Jump-KB")
        )
        {
            // Player is holding down the space bar, accumulate time.
            spaceHeldDownTime += Time.deltaTime;
        }

        Vector2 up = -gravity.DirectionToNearestGround();
        if (debug) Debug.DrawRay(transform.position, up, debugJumpDirectionColor);

        // Jump.
        if (Input.GetButtonUp($"Jump-GP-{gamepadIndex}") || Input.GetButtonUp($"Jump-KB"))
        {
            Debug.Assert(ratioOfInputDirectionJumpImpulseForce >= 0.0f);
            Debug.Assert(ratioOfInputDirectionJumpImpulseForce <= 1.0f);

            gravity.enabled = false;

            // Account for if the player is not choosing a direction.
            float ratioOfInputDirectionFinal = ratioOfInputDirectionJumpImpulseForce;
            if (ignoreInputDirectionJumpForceIfNoInput)
            {
                if (inputMagnitude == 0) ratioOfInputDirectionFinal = 0.0f;
            }

            // Account for if the player's input direction is towards the surface.
            if (ignoreJumpInputDirectionIfInputIsDownwards)
            {
                float dotInputWithUp = up.x * inputDirection.x + up.y * inputDirection.y;
                if (dotInputWithUp < isDownwardsTolerance) ratioOfInputDirectionFinal = 0.0f;
            }
            
            Vector2 upSurfaceDirectionForce = up * jumpBaseImpulseMagnitude * (1.0f + Mathf.Clamp(spaceHeldDownTime, 0.0f, spaceHeldJumpImpulseMaximum)) * (1.0f - ratioOfInputDirectionFinal);
            Vector2 inputDirectionForce = inputDirection * jumpBaseImpulseMagnitude * ratioOfInputDirectionFinal;

            Vector2 jumpForce = upSurfaceDirectionForce + inputDirectionForce;

            rb.AddForce(jumpForce);
            spaceHeldDownTime = 0.0f;
        }
        // No jump, apply gravity.
        else
        {
            gravity.enabled = true;
        }
        #endregion
        #region SplineMovement

        // If no input, reset data and return - nothing to do.
        if (inputMagnitude < float.Epsilon)
        {
            moveInSameDirectionTime = 0.0f;
            previousMovementDirection = MovementDirection.None;
            return;
        }

        // Draw the input direction.
        if(debug) Debug.DrawRay(transform.position, inputDirection * inputMagnitude, debugInputColor);

        // Get the forward direction based on the spline.
        Vector2 forward = gravity.DirectionForwardOnNearestGround();
        if (debug) Debug.DrawRay(transform.position, forward, debugForwardColor);

        // Get the backward direction based on the spline.
        Vector3 backward = -forward;
        if (debug) Debug.DrawRay(transform.position, backward, debugBackwardColor);

        // Get the relative angle from the input direction to the forward and backward directions.
        float dotInputForward = inputDirection.x * forward.x + inputDirection.y * forward.y;
        float dotInputBackward = inputDirection.x * backward.x + inputDirection.y * backward.y;

        // Bias the results towards whatever direction they were previously heading,
        // this allows for leeway in sharp corners.
        if(previousMovementDirection == MovementDirection.Forwards)
        {
            dotInputForward += biasTowardsPreviousDirection;
        } else if(previousMovementDirection == MovementDirection.Backwards)
        {
            dotInputBackward += biasTowardsPreviousDirection;
        }

        // Require overcoming a value before moving, this allows for holding down
        // the direction perpindicular to forward and backward - i.e. holding up on
        // a flat floor - without moving.
        float valueToOvercome = previousMovementDirection == MovementDirection.None ? overcomeBiasBeforeMoving : 0;
        if (dotInputForward > valueToOvercome)
        {
            // Moving Forwards.

            if(previousMovementDirection == MovementDirection.Backwards)
            {
                // Reset direction time as direction changed.
                moveInSameDirectionTime = 0.0f; 
            }
            float acceleration = movementAccelerationCurve.Evaluate(moveInSameDirectionTime);

            // Apply a force in the forward direction.
            Vector2 force = new Vector2(forward.x, forward.y);
            force *= rb.mass * inputMagnitude * acceleration * movementSpeed * Time.deltaTime;
            rb.AddForce(force);
            previousMovementDirection = MovementDirection.Forwards;
            moveInSameDirectionTime += Time.deltaTime;
        }
        else if (dotInputBackward > valueToOvercome)
        {
            // Moving Backwards.

            if (previousMovementDirection == MovementDirection.Forwards)
            {
                // Reset direction time as direction changed.
                moveInSameDirectionTime = 0.0f;
            }
            float acceleration = movementAccelerationCurve.Evaluate(moveInSameDirectionTime);

            // Apply a force in the backward direction.
            Vector2 force = new Vector2(backward.x, backward.y);
            force *= rb.mass * inputMagnitude * acceleration * movementSpeed * Time.deltaTime;
            rb.AddForce(force);
            previousMovementDirection = MovementDirection.Backwards;
            moveInSameDirectionTime += Time.deltaTime;
        }
        #endregion
    }
}
