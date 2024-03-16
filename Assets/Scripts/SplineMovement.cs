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

    public AnimationCurve accelerationCurve;

    public float speed = 5.0f;
    
    public float jumpForce = 100.0f;

    public float spaceHeldDownTime = 0.0f;
    public float maximumSpaceHeldDownTime = 1.0f;
    public float moveInSameDirectionTime = 0.0f; // How long the player has been moving for without stopping.

    // 0->1.0
    public float biasTowardsExistingDirectionAmount = 0.2f;
    MovementDirection previousDirection = MovementDirection.None;

    public float overcomeBiasBeforeMoving = 0.3f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravity = GetComponent<SplineGravity>();
        inputs = GetComponent<Inputs>();
    }

    private void OnEnable()
    {
        spaceHeldDownTime = 0.0f;
        previousDirection = MovementDirection.None;
    }

    void Update()
    {
        int gamepadIndex = inputs.GamepadIndex();
        rb.gravityScale = 0.0f;

        #region Jumping
        if (Input.GetButton($"Jump-GP-{gamepadIndex}") 
            || Input.GetButtonDown($"Jump-GP-{gamepadIndex}")
            || Input.GetButton($"Jump-KB")
            || Input.GetButtonDown($"Jump-KB")
        )
        {
            spaceHeldDownTime += Time.deltaTime;
        }

        Vector2 up = -gravity.DirectionToNearestGround();
        if (debug) Debug.DrawRay(transform.position, up, debugJumpDirectionColor);

        // Jump.
        if (Input.GetButtonUp($"Jump-GP-{gamepadIndex}") || Input.GetButtonUp($"Jump-KB"))
        {
            gravity.enabled = false;

            Vector2 upForce = up * jumpForce * (1.0f + Mathf.Clamp(spaceHeldDownTime, 0.0f, maximumSpaceHeldDownTime));
            rb.AddForce(upForce);
            spaceHeldDownTime = 0.0f;
        }
        // No jump, apply gravity.
        else
        {
            gravity.enabled = true;
        }
        #endregion
        #region SplineMovement

        // Get input.
        float hz = Input.GetAxis($"Horizontal-GP-{gamepadIndex}") + Input.GetAxis($"Horizontal");
        float vt = Input.GetAxis($"Vertical-GP-{gamepadIndex}") + Input.GetAxis($"Vertical");
        
        // If no input, reset data and return - nothing to do.
        if (hz == 0.0f && vt == 0.0f)
        {
            moveInSameDirectionTime = 0.0f;
            previousDirection = MovementDirection.None;
            return;
        }

        // Get the input direction from the player.
        Vector2 inputDirection = new Vector2(hz, vt);
        float inputMagnitude = inputDirection.magnitude;
        inputDirection.Normalize();
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
        if(previousDirection == MovementDirection.Forwards)
        {
            dotInputForward += biasTowardsExistingDirectionAmount;
        } else if(previousDirection == MovementDirection.Backwards)
        {
            dotInputBackward += biasTowardsExistingDirectionAmount;
        }

        // Require overcoming a value before moving, this allows for holding down
        // the direction perpindicular to forward and backward - i.e. holding up on
        // a flat floor - without moving.
        float valueToOvercome = previousDirection == MovementDirection.None ? overcomeBiasBeforeMoving : 0;
        if (dotInputForward > valueToOvercome)
        {
            if(previousDirection == MovementDirection.Backwards)
            {
                // Reset direction time as direction changed.
                moveInSameDirectionTime = 0.0f; 
            }
            float acceleration = accelerationCurve.Evaluate(moveInSameDirectionTime);

            // Apply a force in the forward direction.
            Vector2 force = new Vector2(forward.x, forward.y);
            force *= inputMagnitude * acceleration * speed * Time.deltaTime;
            rb.AddForce(force);
            previousDirection = MovementDirection.Forwards;
            moveInSameDirectionTime += Time.deltaTime;
        }
        else if (dotInputBackward > valueToOvercome)
        {
            if (previousDirection == MovementDirection.Forwards)
            {
                // Reset direction time as direction changed.
                moveInSameDirectionTime = 0.0f;
            }
            float acceleration = accelerationCurve.Evaluate(moveInSameDirectionTime);

            // Apply a force in the backward direction.
            Vector2 force = new Vector2(backward.x, backward.y);
            force *= inputMagnitude * acceleration * speed * Time.deltaTime;
            rb.AddForce(force);
            previousDirection = MovementDirection.Backwards;
            moveInSameDirectionTime += Time.deltaTime;
        }
        #endregion
    }
}
