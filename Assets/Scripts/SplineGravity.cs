using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class SplineGravity : MonoBehaviour
{
    Ground ground;
    Rigidbody2D rb;

    public float power = 1000f;

    public GameObject projectorPrefab;

    public bool debug = false;
    public Color directionToNearestGroundColor = new Color();
    public Color directionForwardOnNearestGroundColor = new Color();

    List<SplineProjector> projectors = new List<SplineProjector>();

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject[] terrains = GameObject.FindGameObjectsWithTag("Terrain");
        foreach (GameObject terrain in terrains)
        {
            SplineComputer spline = terrain.GetComponent<SplineComputer>();
            Debug.Assert(spline != null);

            GameObject projectorGO = GameObject.Instantiate(projectorPrefab);
            projectorGO.transform.parent = transform;
            SplineProjector projector = projectorGO.GetComponent<SplineProjector>();
            projector.projectTarget = transform;
            projector.spline = spline;
            projectors.Add(projector);
        }
    }

    private void Update()
    {
        if (debug)
        {
            Vector2 directionToNearestGround = DirectionToNearestGround();
            Debug.DrawRay(transform.position, directionToNearestGround, directionToNearestGroundColor);

            Vector2 directionForwardOnNearestGround = DirectionForwardOnNearestGround();
            Debug.DrawRay(transform.position, directionForwardOnNearestGround, directionForwardOnNearestGroundColor);
        }
    }

    public SplineProjector NearestGround()
    {
        SplineProjector nearestGroundProjector = null;
        float minimumDistanceToGround = float.MaxValue;
        foreach (SplineProjector projector in projectors)
        {
            Vector3 toGround = (projector.result.position - transform.position);
            float distanceToGround = toGround.magnitude;
            if (distanceToGround < minimumDistanceToGround)
            {
                minimumDistanceToGround = distanceToGround;
                nearestGroundProjector = projector;
            }
        }
        Debug.Assert(nearestGroundProjector != null);
        Debug.Assert(minimumDistanceToGround < float.MaxValue);

        return nearestGroundProjector;
    }

    public float DistanceToNearestGround()
    {
        float radius = transform.localScale.x / 2.0f;
        float minimumDistanceToGround = float.MaxValue;
        foreach (SplineProjector projector in projectors)
        {
            Vector3 toGround = (projector.result.position - transform.position);
            float distanceToGround = toGround.magnitude;
            if (distanceToGround < minimumDistanceToGround)
            {
                minimumDistanceToGround = distanceToGround;
            }
        }
        Debug.Assert(minimumDistanceToGround < float.MaxValue);

        return Mathf.Clamp(minimumDistanceToGround - radius, 0.0f, float.MaxValue);
    }

    public Vector2 DirectionToNearestGround()
    {
        SplineProjector nearestGround = NearestGround();
        Vector3 down = nearestGround.result.right;
        Vector2 quantizedDown = new Vector2(down.x, down.y);
        quantizedDown.Normalize();
        return quantizedDown;
    }

    public Vector2 DirectionForwardOnNearestGround()
    {
        SplineProjector nearestGround = NearestGround();
        Vector3 down = nearestGround.result.forward;
        Vector2 quantizedDown = new Vector2(down.x, down.y);
        quantizedDown.Normalize();
        return quantizedDown;
    }

    void LateUpdate()
    {
        rb.gravityScale = 0.0f;

        // Apply Gravity towards terrain.
        Vector2 down = DirectionToNearestGround();
        down.Normalize();
        Vector2 downForce = down * power;
        rb.AddForce(downForce * Time.deltaTime);
    }
}
