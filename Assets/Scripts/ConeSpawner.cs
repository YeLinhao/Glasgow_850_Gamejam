using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject orangeConePrefab;
    [SerializeField] private GameObject blueConePrefab;
    [SerializeField] private GameObject greenConePrefab;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxCones = 3;
    [SerializeField] private float spawnRange = 10f;
    [SerializeField] private float minDistanceFromOrigin = 2f;
    [SerializeField] private float spawnY = 1f;

    private List<GameObject> conesInPlay = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            conesInPlay.RemoveAll(c => c == null);

            if (conesInPlay.Count < maxCones)
            {
                Vector3 spawnPos = GetRandomSpawnPosition();
                GameObject prefab = ChooseConePrefab();
                GameObject cone = Instantiate(prefab, spawnPos, Quaternion.identity);

                // 25% chance to enable light
                if (Random.value < 0.25f)
                {
                    //var coneScript = cone.GetComponent<Cone>();
                    //if (coneScript != null)
                    //    coneScript.hasLight = true;
                }

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
            if (attempts > 100) break;
        }
        while (Vector2.Distance(new Vector2(pos.x, pos.z), Vector2.zero) < minDistanceFromOrigin);

        return pos;
    }

    private GameObject ChooseConePrefab()
    {
        float rand = Random.value;

        if (rand < 0.05f) return greenConePrefab;  // 5%
        else if (rand < 0.25f) return blueConePrefab; // next 20%
        else return orangeConePrefab; // remaining 75%
    }
}
