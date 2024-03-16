using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterfall : MonoBehaviour
{
    public GameObject spawnPrefab;

    public float spawnConeRadius = 30.0f;

    public float minimumSpawnRateDelay = 0.5f;
    public float maximumSpawnRateDelay = 1.0f;

    public float minimumSpawnImpulseMagnitude = 10.0f;
    public float maximumSpawnImpulseMagnitude = 20.0f;

    float nextSpawnTime = 0.0f;

    // Update is called once per frame
    void Update()
    {
        nextSpawnTime -= Time.deltaTime;
        if(nextSpawnTime < 0.0f)
        {
            GameObject spawned = GameObject.Instantiate(spawnPrefab);
            spawned.transform.position = transform.position;

            float forwardAngle = transform.rotation.eulerAngles.z;
            print(forwardAngle);

            Rigidbody2D rb = spawned.GetComponent<Rigidbody2D>();
            float randomCone = Random.Range(-spawnConeRadius / 2, spawnConeRadius / 2);
            Vector2 impulse = new Vector2(Mathf.Cos(forwardAngle + randomCone), Mathf.Sin(forwardAngle + randomCone));
            impulse *= Random.Range(minimumSpawnImpulseMagnitude, maximumSpawnImpulseMagnitude);
            rb.AddForce(impulse);

            nextSpawnTime = minimumSpawnRateDelay + Random.Range(0, maximumSpawnRateDelay);
        }
    }
}
