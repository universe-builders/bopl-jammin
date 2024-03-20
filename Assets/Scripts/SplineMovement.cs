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

    SplineGravity gravity;
    Rigidbody2D rb;
    Inputs inputs;
    MovementStateMachine movementStateMachine;
    States states;

    public Vector2 previousTarget = Vector2.zero;

    [Tooltip("This curve is used to dictate the acceleration of the movement of the player.")]
    public AnimationCurve movementAccelerationCurve;

    [Tooltip("This is applied to the movement force each frame.")]
    public float movementSpeed = 300f;

    [Tooltip("How long the player has been moving for without stopping. Used to calculate the movement acceleration.")]
    public float moveInSameDirectionTime = 0.0f;

    [Tooltip("[0->1] How much to bias towards the previous direction, this means the movement is more likely to use the previous direction than a new direction which helps with sharp corners and provides time to allow the player to react.")]
    public float biasTowardsPreviousDirection = 0.6f;

    [Tooltip("The previous movement direction of the player. Used to bias towards same direction or new direction.")]
    public MovementDirection previousMovementDirection = MovementDirection.None;

    [Tooltip("[0 -> 1] How close to the forward/backward direction before starting to move. This allows the player holding down the perpindicular direction to forward without moving.")]
    public float overcomeBiasBeforeMoving = 0.3f;

    public float linearDragWhenMoving = 0.0f;
    public float linearDragWhenFinishedMoving = 100.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravity = GetComponent<SplineGravity>();
        inputs = GetComponent<Inputs>();
        movementStateMachine = GetComponent<MovementStateMachine>();
        states = GetComponent<States>();
    }

    private void OnEnable()
    {
        if (rb == null) return;
        previousMovementDirection = MovementDirection.None;


        





    }

    private void OnDisable()
    {
        //rb.drag = linearDragWhenFinishedMoving;
    }

    void Update()
    {
        // Don't use gravity. Gravity is a real downer.
        rb.gravityScale = 0.0f;

        // Get input.
        Vector2 inputDirection = inputs.GetInputDirection();
        float inputMagnitude = inputs.GetInputMagnitude();

        previousTarget = gravity.NearestGround().result.position;

        // If no input, reset data and return - nothing to do.
        if (inputMagnitude < float.Epsilon)
        {
            //rb.drag = linearDragWhenFinishedMoving;
            //moveInSameDirectionTime = 0.0f;
            //previousMovementDirection = MovementDirection.None;
            rb.velocity = Vector2.zero;
            return;
        }

        // Draw the input direction.
        if (debug) Debug.DrawRay(transform.position, inputDirection * inputMagnitude, debugInputColor);

        // Get the forward direction based on the spline.
        Vector2 forward = gravity.DirectionForwardOnNearestGround();
        if (debug) Debug.DrawRay(transform.position, forward, debugForwardColor);

        // Get the backward direction based on the spline.
        Vector3 backward = -forward;
        if (debug) Debug.DrawRay(transform.position, backward, debugBackwardColor);

        // Get the relative angle from the input direction to the forward and backward directions.
        float dotInputForward = inputDirection.x * forward.x + inputDirection.y * forward.y;
        float dotInputBackward = inputDirection.x * backward.x + inputDirection.y * backward.y;

        float acceleration = 1.0f;// movementAccelerationCurve.Evaluate(moveInSameDirectionTime);
        Vector2 velocity = rb.velocity;
        float velocityMagnitude = velocity.magnitude;

        // Bias the results towards whatever direction they were previously heading,
        // this allows for leeway in sharp corners.
        if (previousMovementDirection == MovementDirection.Forwards)
        {
            dotInputForward += biasTowardsPreviousDirection;
        }
        else if (previousMovementDirection == MovementDirection.Backwards)
        {
            dotInputBackward += biasTowardsPreviousDirection;
        }

        // Require overcoming a value before moving, this allows for holding down
        // the direction perpindicular to forward and backward - i.e. holding up on
        // a flat floor - without moving.
        float valueToOvercome = previousMovementDirection == MovementDirection.None ? overcomeBiasBeforeMoving : 0;
        if (dotInputForward > valueToOvercome)
        {
            //float dotWithCurrentVelocity = velocity.x * forward.x + velocity.y * forward.y;
            //if(dotWithCurrentVelocity < )


            // Moving Forwards.

            if (previousMovementDirection != MovementDirection.Forwards)
            {
                //rb.velocity = Vector2.zero;
                // Reset direction time as direction changed.
                //moveInSameDirectionTime = 0.0f;
            }
            
            /*
            float targetVelocity = acceleration * movementSpeed * Time.deltaTime;
            moveInSameDirectionTime += Time.deltaTime;
            rb.velocity = forward * targetVelocity;
            previousMovementDirection = MovementDirection.Forwards;
            */

            //rb.drag = linearDragWhenMoving;

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

            if (previousMovementDirection != MovementDirection.Backwards)
            {
                //rb.velocity = Vector2.zero;
                // Reset direction time as direction changed.
                //moveInSameDirectionTime = 0.0f;
            }

            /*
            float targetVelocity = acceleration * movementSpeed * Time.deltaTime;
            moveInSameDirectionTime += Time.deltaTime;
            rb.velocity = backward * targetVelocity;
            previousMovementDirection = MovementDirection.Backwards;
            */

            //rb.drag = linearDragWhenMoving;
            // Apply a force in the backward direction.
            Vector2 force = new Vector2(backward.x, backward.y);
            force *= rb.mass * inputMagnitude * acceleration * movementSpeed * Time.deltaTime;
            rb.AddForce(force);
            previousMovementDirection = MovementDirection.Backwards;
            moveInSameDirectionTime += Time.deltaTime;

        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
}
