using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlamMovement : MonoBehaviour
{
    Rigidbody2D body;
    SplineGravity gravity;
    float initialGravityScale;
    float initialMass;
    float initialDrag;
    float initialSplineGravityPower;
    States states;

    SplineMovement splineMovement;

    public float gravityScale = 20.0f;
    public float mass = 100.0f;
    public float drag = 1000.0f;
    public float splineGravityPower = 5000.0f;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        states = GetComponent<States>();
        gravity = GetComponent<SplineGravity>();
    }

    private void OnEnable()
    {
        initialGravityScale = body.gravityScale;
        initialMass = body.mass;
        initialDrag = body.drag;
        initialSplineGravityPower = gravity.power;

        body.gravityScale = gravityScale;
        body.mass = mass;
    }

    private void OnDisable()
    {
        body.gravityScale = initialGravityScale;
        body.mass = initialMass;
        body.drag = initialDrag;
        gravity.power = initialSplineGravityPower;
    }

    void Update()
    {
        // prolly need an acceleration curve.

        /*
        Vector2 previousGroundTarget = splineMovement.previousTarget;
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        Vector2 toPreviousGroundTarget = (previousGroundTarget - position);

        Vector2 force *= body.mass * inputMagnitude * acceleration * movementSpeed * Time.deltaTime;
        rb.AddForce(force);
        previousMovementDirection = MovementDirection.Forwards;
        moveInSameDirectionTime += Time.deltaTime;
        */

        if (states.IsOnGround())
        {
            body.gravityScale = initialGravityScale;
            //body.drag = drag;
            gravity.enabled = true;
            gravity.power = splineGravityPower;
        }
    }
}
