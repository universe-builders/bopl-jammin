using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirMovement : MonoBehaviour
{
    CircleCollider2D collider;
    SplineMovement splineMovement;
    Rigidbody2D rb;
    Inputs inputs;

    public float speed = 100.0f;

    public bool debug;
    public Color debugInputColor;

    private void Start()
    {
        collider = GetComponent<CircleCollider2D>();
        splineMovement = GetComponent<SplineMovement>();
        rb = GetComponent<Rigidbody2D>();
        inputs = GetComponent<Inputs>();
    }

    void Update()
    {
        int gamepadIndex = inputs.GamepadIndex();
        rb.gravityScale = 1.0f;

        float hz = Input.GetAxisRaw($"Horizontal-GP-{gamepadIndex}") + Input.GetAxisRaw($"Horizontal");
        if (hz == 0.0f) return; // No input.

        Vector2 inputDirection = new Vector2(hz, 0);
        inputDirection.Normalize();
        if (debug) Debug.DrawRay(transform.position, inputDirection, debugInputColor);

        rb.AddForce(inputDirection * speed * Time.deltaTime);
    }
}
