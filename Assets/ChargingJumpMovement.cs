using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingJumpMovement : MonoBehaviour
{
    [Tooltip("How long the space button has been held down, whilst on the ground. Used to add an extra force to the jump.")]
    public float spaceHeldDownTime = 0.0f;

    [Tooltip("The clamp for for spaceHeldDownTime when applying to the jump force.")]
    public float spaceHeldJumpImpulseMaximum = 1.0f;

    [Tooltip("This is applied to the jump impulse.")]
    public float jumpBaseImpulseMagnitude = 400.0f;

    [Tooltip("[0 -> 1] The jump impulse consists of two forces, the normal of the surface and the direction of the input. At 0.0 the direction of input has no influence, at 1.0 the normal of the surface has no influence.")]
    public float ratioOfInputDirectionJumpImpulseForce = 0.5f;

    [Tooltip("If true, when player is not pressing any input then the full basis force will apply based on surface normal only.")]
    public bool ignoreInputDirectionJumpForceIfNoInput = true;

    [Tooltip("If true, when the player's input is directed towards inside the surface then ignore the input direction in the jump force calculation.")]
    public bool ignoreJumpInputDirectionIfInputIsDownwards = true;

    [Tooltip("[1.0 -> -1.0] Used for ignoring input direction in jump impulse if input is facing surface. At 0.0 then perpindicular angles are considered down, at -1.0 then only the vector exactly down is considered down. Between 0.0 and -1.0 will progressively get more 'down', whereas 0.0 to 1.0 is progressively more 'up'.")]
    public float isDownwardsTolerance = -0.6f;

    public bool debug = false;
    public Color debugJumpDirectionColor;

    public float linearDragWhenJumping = 0.0f;

    public float lastJumpTime = 0.0f;

    public Vector2 lastJumpDirection;
    public Vector2 lastJumpPosition;

    Rigidbody2D rb;
    Inputs inputs;
    SplineGravity gravity; //TMP
    MovementStateMachine movementStateMachine;
    States states;
    public Transform lastJumpObject;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        inputs = GetComponent<Inputs>();
        gravity = GetComponent<SplineGravity>();
        movementStateMachine = GetComponent<MovementStateMachine>();
        states = GetComponent<States>();
    }

    private void OnEnable()
    {
        // Reset variables as was not on the ground before this frame.
        spaceHeldDownTime = 0.0f;
        
    }

    void Update()
    {
        // Don't use gravity. Gravity is a real downer.
        rb.gravityScale = 0.0f;

        // Get input.
        int gamepadIndex = inputs.GamepadIndex();
        Vector2 inputDirection = inputs.GetInputDirection();
        float inputMagnitude = inputs.GetInputMagnitude();

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
            float ratioOfInputDirectionFinal = ratioOfInputDirectionJumpImpulseForce * inputMagnitude;
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

            lastJumpDirection = jumpForce.normalized;
            lastJumpPosition = transform.position;

            rb.drag = linearDragWhenJumping;

            rb.AddForce(jumpForce);
            spaceHeldDownTime = 0.0f;

            movementStateMachine.TransitionTo(MovementState.AirMovement);

            Transform nearestGround = gravity.NearestGround().spline.transform;
            nearestGround.GetComponent<Rigidbody2D>().AddForce(-jumpForce * 100);

            lastJumpObject = gravity.NearestGround().spline.transform;

            lastJumpTime = Time.time;
        }
    }
}
