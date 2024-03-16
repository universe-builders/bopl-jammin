using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnOnCollisionWithDeathZone : MonoBehaviour
{
    PlayerSpawner playerSpawner;

    private void Start()
    {
        playerSpawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<PlayerSpawner>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.transform.tag);
        if(collision.transform.tag == "DeathZone")
        {
            //playerSpawner.RespawnPlayers();
        }
    }
}
