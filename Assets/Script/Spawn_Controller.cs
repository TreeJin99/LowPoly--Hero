using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Controller : MonoBehaviour
{
    public GameObject[] enemies;
    private float currentTime = 0.0f;

    private float alpha = 0.0f;

    private void Start()
    {
        InvokeRepeating("SpawnEnemies", 3, 5);
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject spawnEneies = Instantiate(enemies[0], GetRandomLocate(), transform.rotation);
        }

        for (int i = 0; i < 3; i++)
        {
            GameObject spawnEneies = Instantiate(enemies[1], GetRandomLocate(), transform.rotation);
        }

        for (int i = 0; i < 2; i++)
        {
            GameObject spawnEneies = Instantiate(enemies[2], GetRandomLocate(), transform.rotation);
        }
    }


    private Vector3 GetRandomLocate()
    {
        Vector3 RandomLocate = new Vector3(Random.Range(-25f, 25f), 0.25f, Random.Range(-25f, 25f));

        return RandomLocate;
    }
}
