using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInCircle : MonoBehaviour
{
    public float minimumRadius = 1.0f;
    public float maximumRadius = 10.0f;
    public float radiusChangeSpeed = 1.0f;

    public float speed = 10.0f;

    [SerializeField]
    float angle = 0.0f;

    [SerializeField]
    float distance = 0.0f;
    
    void Update()
    {
        angle += speed * Time.deltaTime;

        float radiusChange = radiusChangeSpeed * Time.deltaTime;
        float posynegy = Random.Range(0, 2);
        if(posynegy < 1)
        {
            distance += radiusChange;
        }
        else
        {
            distance -= radiusChange;
        }
        distance = Mathf.Clamp(distance, minimumRadius, maximumRadius);

        Vector2 position = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * distance;
        transform.position = position;
    }
}
