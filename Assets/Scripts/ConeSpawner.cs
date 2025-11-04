using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject orangeConePrefab;
    [SerializeField] private GameObject blueConePrefab;
    [SerializeField] private GameObject greenConePrefab;
    [SerializeField] private GameObject IrnBruPrefab;
    [SerializeField] private GameObject BeerPrefab;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float itemSpawnInterval = 10f;
    [SerializeField] private int maxCones = 3;
    [SerializeField] private float spawnRange = 10f;
    [SerializeField] private float minDistanceFromOrigin = 2f;
    [SerializeField] private float spawnY = 1f;


    private List<GameObject> conesInPlay = new List<GameObject>();
    private List<GameObject> itemsInPlay = new List<GameObject>();
    private bool spawnStarted = false;
    private bool itemSpawnStarted = false;

    void Start()
    {
        
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
    private IEnumerator ItemSpawnRoutine()
    {
        while (true)
        {
            itemsInPlay.RemoveAll(c => c == null);

            if (itemsInPlay.Count < maxCones)
            {
                Vector3 spawnPos = GetRandomSpawnPosition();
                GameObject prefab = ChooseItemPrefab();
                GameObject item = Instantiate(prefab, spawnPos, Quaternion.identity);

                // 25% chance to enable light
                if (Random.value < 0.25f)
                {
                    //var coneScript = cone.GetComponent<Cone>();
                    //if (coneScript != null)
                    //    coneScript.hasLight = true;
                }

                itemsInPlay.Add(item);
            }

            yield return new WaitForSeconds(itemSpawnInterval);
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

    private GameObject ChooseItemPrefab()
    {
        float rand = Random.value;

        if (rand < 0.5f) return IrnBruPrefab;
        else return BeerPrefab; 
    }
    private void Update()
    {
        if (GameManager.CurrentState == GameManager.GameState.MainGame && spawnStarted == false)
        {
            StartCoroutine(SpawnRoutine());
            spawnStarted = true;
        }
        if (TimerWithTMPro.currentTime <= 70 && itemSpawnStarted == false)
        {
            StartCoroutine(ItemSpawnRoutine());
            itemSpawnStarted = true;
        }
    }
    }
