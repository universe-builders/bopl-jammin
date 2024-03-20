using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    Rigidbody body;
    PolygonCollider2D collider;

    public List<Transform> edibleSlimes = new List<Transform>();

    public float health;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        collider = GetComponent<PolygonCollider2D>();

        Transform[] transforms = transform.GetComponentsInChildren<Transform>();
        foreach (Transform child in transforms)
        {
            if (child.tag == "UnsentientSlime")
            {
                edibleSlimes.Add(child);
                child.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        if(health <= 0)
        {
            foreach(Transform edibleSlime in edibleSlimes)
            {
                edibleSlime.gameObject.SetActive(true);
                edibleSlime.transform.parent = transform.parent;
                edibleSlime.GetComponent<Collider2D>().enabled = true;
            }

            GameObject.Destroy(gameObject);
        }        
    }
}
