using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] public GameObject conePrefab;
    [SerializeField] public float spawnInterval = 2f;   // seconds between spawns
    [SerializeField] public int maxCones = 3;
    [SerializeField] public float spawnRange = 10f;     // random range for x and z
    [SerializeField] public float minDistanceFromOrigin = 2f;
    [SerializeField] public float spawnY = 1f;

    private List<GameObject> conesInPlay = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Remove null cones that were destroyed
            conesInPlay.RemoveAll(c => c == null);

            if (conesInPlay.Count < maxCones)
            {
                Vector3 spawnPos = GetRandomSpawnPosition();
                GameObject cone = Instantiate(conePrefab, spawnPos, Quaternion.identity);
                conesInPlay.Add(cone);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 pos;
        int attempts = 0;

        do
        {
            float x = Random.Range(-spawnRange, spawnRange);
            float z = Random.Range(-spawnRange, spawnRange);

            pos = new Vector3(x, spawnY, z);

            attempts++;
            if (attempts > 100) break; // fail-safe
        }
        while (Vector2.Distance(new Vector2(pos.x, pos.z), Vector2.zero) < minDistanceFromOrigin);

        return pos;
    }
}
