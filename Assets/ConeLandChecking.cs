using System.Collections.Generic;
using UnityEngine;


public class ConeLandChecking : MonoBehaviour
{
    [SerializeField] private List<GameObject> cones = new List<GameObject>();
    [SerializeField] private bool loop = false; // set in Inspector: if true, wraps to start after last
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private GameObject orangeConeMeshPrefab;
    [SerializeField] private GameObject blueConeMeshPrefab;
    [SerializeField] private GameObject greenConeMeshPrefab;
    private int nextIndex = 0;
    private float spawnYOffset = 0f; // keeps track of Y increment



    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object has a specific tag or component if needed
        if (other.CompareTag("cone") || other.CompareTag("blueCone") || other.CompareTag("greenCone"))
        {
            Debug.Log("A cone entered the trigger!");
            
            AddConeOnDuke(other.gameObject);
            Destroy(other.gameObject);
        }
    }


    private void AddConeOnDuke()
    {
        if (cones == null || cones.Count == 0) return;

        if (nextIndex >= cones.Count)
        {
            if (loop) nextIndex = 0;
            else return; // no more cones and not looping
        }

        GameObject cone = cones[nextIndex];
        if (cone != null && !cone.activeInHierarchy)
        {
            cone.SetActive(true);
        }

        nextIndex++;

    }

    private void AddConeOnDuke(GameObject destroyedCone)
    {
        // Determine which prefab to spawn
        GameObject prefabToSpawn = orangeConeMeshPrefab;
        int score = 1;

        // You could also use a tag, layer, or a component property to identify
        if (destroyedCone.CompareTag("blueCone"))
        {
            prefabToSpawn = blueConeMeshPrefab;
            score = 3;
        }
        else if (destroyedCone.CompareTag("greenCone"))
        {
            prefabToSpawn = greenConeMeshPrefab;
            score = 5;
        }
        // else defaults to orange

        // Calculate spawn position
        Vector3 spawnPos = spawnPoint.position + new Vector3(0f, spawnYOffset, 0f);
        spawnYOffset += 0.2f; // increment Y for next spawn

        // Instantiate the cone
        GameManager.instance.addScore(destroyedCone.gameObject.GetComponent<PickupableItem>().owner, score);
        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
    }

}
