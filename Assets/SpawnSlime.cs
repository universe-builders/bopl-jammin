using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class SpawnSlime : MonoBehaviour
{
    public float minimumTimeBetweenSpawns = 0.0f;
    public float maximumTimeBetweenSpawns = 5.0f;

    public GameObject eadibleSlimePrefab;

    public List<SplineProjector> projectors = new List<SplineProjector>();

    [SerializeField]
    float timeTillSpawn = 0.0f;

    private void Start()
    {
        timeTillSpawn = maximumTimeBetweenSpawns;
    }

    private void Update()
    {
        timeTillSpawn -= Time.deltaTime;

        if(timeTillSpawn < 0.0f)
        {
            Debug.Assert(projectors.Count > 0);
            float minimumDistance = float.MaxValue;
            SplineProjector minimumProjector = null;
            foreach(SplineProjector projector in projectors)
            {
                Vector3 toGround = (projector.result.position - transform.position);
                float distanceToGround = toGround.magnitude;
                if (distanceToGround < minimumDistance)
                {
                    minimumDistance = distanceToGround;
                    minimumProjector = projector;
                }
            }            

            GameObject edibleSlime = GameObject.Instantiate(eadibleSlimePrefab);
            

            Vector3 surfacePos = minimumProjector.result.position;
            Vector3 fromSurface = (transform.position - surfacePos).normalized;
            float radius = edibleSlime.transform.localScale.x / 2;
            Vector3 spawnPos = surfacePos + (fromSurface * radius);
            edibleSlime.transform.position = spawnPos;

            timeTillSpawn = Random.Range(minimumTimeBetweenSpawns, maximumTimeBetweenSpawns);
        }
    }


}
