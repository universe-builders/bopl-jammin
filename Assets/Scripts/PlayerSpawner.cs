using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public List<Material> playerMaterials = new List<Material>(); 

    HashSet<int> SpawnedPlayersGamepadIndicies()
    {
        HashSet<int> spawnedPlayersGamepadIndicies = new HashSet<int>();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Inputs inputs = player.GetComponent<Inputs>();
            Debug.Assert(inputs != null);

            int gamepadIndex = inputs.GamepadIndex();
            spawnedPlayersGamepadIndicies.Add(gamepadIndex);
        }

        return spawnedPlayersGamepadIndicies;
    }

    void Update()
    {
        if (Input.GetButtonDown("Spawn-GP-0") || Input.GetButtonDown("Spawn-KB"))
        {
            if (!SpawnedPlayersGamepadIndicies().Contains(0)) SpawnPlayer(0);
        }
        else if (Input.GetButtonDown("Spawn-GP-1"))
        {
            if (!SpawnedPlayersGamepadIndicies().Contains(1)) SpawnPlayer(1);
        }
        else if (Input.GetButtonDown("Spawn-GP-2"))
        {
            if (!SpawnedPlayersGamepadIndicies().Contains(2)) SpawnPlayer(2);
        }
        else if (Input.GetButtonDown("Spawn-GP-3"))
        {
            if (!SpawnedPlayersGamepadIndicies().Contains(3)) SpawnPlayer(3);
        }
    }

    void SpawnPlayer(int gamepadIndex)
    {
        Debug.Assert(!SpawnedPlayersGamepadIndicies().Contains(gamepadIndex));

        GameObject player = GameObject.Instantiate(playerPrefab);

        // Find position to spawn.
        GameObject[] playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");
        Debug.Assert(gamepadIndex < playerSpawns.Length);
        player.transform.position = playerSpawns[gamepadIndex].transform.position;

        // Set input gamepad index.
        Inputs inputs = player.GetComponent<Inputs>();
        inputs.SetGamepadIndex(gamepadIndex);

        // Set material.
        Debug.Assert(gamepadIndex < playerMaterials.Count);
        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
        spriteRenderer.material = playerMaterials[gamepadIndex];
    }
}
