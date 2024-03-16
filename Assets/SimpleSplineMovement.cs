using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSplineMovement : MonoBehaviour
{
    SplineGravity gravity;
    Rigidbody2D body;

    public float initialImpulseFactor = 10.0f;

    public bool forward = false;
    public float speed = 100.0f;

    private void Start()
    {
        gravity = GetComponent<SplineGravity>();
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 forward = gravity.DirectionForwardOnNearestGround();
        body.AddForce(initialImpulseFactor * forward * speed * Time.deltaTime);

        initialImpulseFactor = 1.0f;
    }
}
