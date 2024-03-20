using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Inputs inputs;
    MovementStateMachine movementStateMachine;
    States states;
    ChargingJumpMovement jump;
    SplineGravity gravity;

    public float speed = 100.0f;
    public float gravityScale = 2.0f;
    public float descendingGravityScale = 4.0f;

    public bool increaseGravityWhenDescending = true;

    public bool debug;
    public Color debugInputColor;

    public float linearDrag = 1.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        inputs = GetComponent<Inputs>();
        movementStateMachine = GetComponent<MovementStateMachine>();
        states = GetComponent<States>();
        jump = GetComponent<ChargingJumpMovement>();
        gravity = GetComponent<SplineGravity>();
    }

    private void OnDisable()
    {
        rb.gravityScale = gravityScale;
        
    }
   

    void ConserveMomentum()
    {
        // Turn forces into the input direction.
        Vector2 velocity = rb.velocity;
        float velocityMagnitude = rb.velocity.magnitude;

        /*
        Vector2 down = gravity.DirectionToNearestGround();
        float dotVelocityAndUp = down.x * velocity.x + down.y * velocity.y;
        if (dotVelocityAndUp > 0.7f)
        {
            // Landing on wall where velocity is pointed downwards,
            // so stick to wall without transfering velocity.
            //rb.velocity = Vector2.zero;
            return;
        }
        */

        Vector2 forward = gravity.DirectionForwardOnNearestGround();
        Vector2 backward = -forward;

        Vector2 inputDirection = inputs.GetInputDirection();
        float inputMagnitude = inputs.GetInputMagnitude();
        if (inputMagnitude != 0.0f)
        {
            float dotInputForward = inputDirection.x * forward.x + inputDirection.y * forward.y;
            float dotInputBackward = inputDirection.x * backward.x + inputDirection.y * backward.y;

            if (dotInputForward > dotInputBackward)
            {
                Vector2 force = forward * velocityMagnitude;
                rb.AddForce(force);
            }
            else
            {
                // Apply a force in the forward direction.
                Vector2 force = backward * velocityMagnitude;
                rb.AddForce(force);
            }
        }
        else
        {
            float dotInputForward = velocity.x * forward.x + velocity.y * forward.y;
            float dotInputBackward = velocity.x * backward.x + velocity.y * backward.y;

            if (dotInputForward > dotInputBackward)
            {
                Vector2 force = forward * velocityMagnitude;
                rb.AddForce(force);
            }
            else
            {
                // Apply a force in the forward direction.
                Vector2 force = backward * velocityMagnitude;
                rb.AddForce(force);
            }
        }

    }

    void Update()
    {

        if (states.IsOnGround())
        {
            Transform nearestGround = gravity.NearestGround().spline.transform;
            if(nearestGround != jump.lastJumpObject)
            {
                // Ground is far from the jump ground, prolly a new ground then!
                movementStateMachine.TransitionTo(MovementState.GroundMovement);
                ConserveMomentum();
                return;
            }
        }

        if (states.IsOnGround() && (Time.time - jump.lastJumpTime) > 0.01)
        {
            Vector2 ground = gravity.NearestGround().result.position;
            if( (ground - jump.lastJumpPosition).magnitude > 0.5f)
            {
                // Ground is far from the jump ground, prolly a new ground then!
                movementStateMachine.TransitionTo(MovementState.GroundMovement);
                ConserveMomentum();
                return;
            }


            Vector2 up = -gravity.DirectionToNearestGround();
            Vector2 jumpDir = jump.lastJumpDirection;
            float dot = up.x * jumpDir.x + up.y * jumpDir.y;
            if(dot < 0.75f)
            {
                // The ground that i'm touching is made of lava, but also
                // it is a very different face than the one I jumped from,
                // so prolly a new ground!
                movementStateMachine.TransitionTo(MovementState.GroundMovement);
                ConserveMomentum();
                return;
            }
        }



        rb.drag = linearDrag;
        rb.gravityScale = gravityScale;

        if (increaseGravityWhenDescending && states.IsDescending())
        {
            rb.gravityScale = descendingGravityScale;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }

        Vector2 inputDirection = inputs.GetInputDirection();
        float inputMagnitude = inputs.GetInputMagnitude();
        Vector2 input = inputDirection * inputMagnitude;
        if (debug) Debug.DrawRay(transform.position, input, debugInputColor);

        rb.AddForce(input * speed * Time.deltaTime);
    }
}
