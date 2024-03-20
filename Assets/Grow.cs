using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grow : MonoBehaviour
{
    public float unsentientSlimeGrowthRatio = 1.0f;

    public float slimeGrowthRatio = 0.5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "UnsentientSlime")
        {
            float unsentientSlimeSize = collision.transform.localScale.x;
            float growthAmount = unsentientSlimeSize * unsentientSlimeGrowthRatio;
            transform.localScale += new Vector3(growthAmount, growthAmount, growthAmount);
            GameObject.Destroy(collision.gameObject);
        }
        else if(collision.transform.tag == "Player")
        {
            float otherRadius = collision.transform.localScale.x / 2;
            float radius = transform.localScale.x / 2;

            if(radius > otherRadius)
            {
                float growthAmount = otherRadius * slimeGrowthRatio;
                transform.localScale += new Vector3(growthAmount, growthAmount, growthAmount);
                GameObject.Destroy(collision.gameObject);
            }
        }
    }
}
