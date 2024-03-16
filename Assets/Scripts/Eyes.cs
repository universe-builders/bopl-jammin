using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RayResult
{
    public bool didHit;
    public float angle;
    public RaycastHit2D? hitInfo;
}

public class Eyes : MonoBehaviour
{
    public uint numberOfRays = 180;
    public float lengthOfRays = 10.0f;

    public List<RayResult> rayResults = new List<RayResult>();
    public int closestTerrainRayIndex = -1;

    public RayResult? GetClosestSeenTerrainRay()
    {
        if (closestTerrainRayIndex == -1) return null;
        return rayResults[closestTerrainRayIndex];
    }

    void Update()
    {
        Debug.Assert(numberOfRays > 0);
        Debug.Assert(lengthOfRays % 2 == 0);

        // Reset data.
        rayResults.Clear();
        closestTerrainRayIndex = -1;

        float closestTerrainDistance = float.PositiveInfinity;
        float angle_between_rays = 360.0f / numberOfRays;
        for (int rayIndex = 0; rayIndex < numberOfRays; ++rayIndex)
        {
            // Compute ray.
            float rayAngle = angle_between_rays * rayIndex;
            float rayAngleRadians = Mathf.Deg2Rad * rayAngle;
            Vector3 rayDirection = new Vector3(Mathf.Cos(rayAngleRadians), Mathf.Sin(rayAngleRadians), 0f);
             
            // Perform raycast.
            int layerMask = ~(1 << 7);
            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, rayDirection, lengthOfRays, layerMask);

            // Store ray result.
            RayResult rayResult = new RayResult();
            rayResult.didHit = hitInfo.transform != null;
            if (rayResult.didHit)
            {
                rayResult.hitInfo = hitInfo;
                rayResult.angle = rayAngle;
            }
            rayResults.Add(rayResult);

            // Closest Terrain.
            if (rayResult.didHit)
            {
                if(rayResult.hitInfo.Value.transform.tag == "Terrain")
                {
                    if(rayResult.hitInfo.Value.distance < closestTerrainDistance)
                    {
                        closestTerrainDistance = rayResult.hitInfo.Value.distance;
                        closestTerrainRayIndex = rayIndex;
                    }
                }
            }

            // Render ray.
            if (rayResult.didHit)
            {
                Debug.DrawLine(transform.position, hitInfo.point, Color.green);
            }
            else
            {
                Vector3 rayEnd = transform.position + (rayDirection * lengthOfRays);
                Debug.DrawLine(transform.position, rayEnd, Color.red);
            }


        }
    }
}
