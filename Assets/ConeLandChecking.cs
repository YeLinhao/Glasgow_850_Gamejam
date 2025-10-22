using System.Collections.Generic;
using UnityEngine;


public class ConeLandChecking : MonoBehaviour
{
    [SerializeField] private List<GameObject> cones = new List<GameObject>();
    [SerializeField] private bool loop = false; // set in Inspector: if true, wraps to start after last
    private int nextIndex = 0;
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object has a specific tag or component if needed
        if (other.CompareTag("cone"))
        {
            Debug.Log("A cone entered the trigger!");
            Destroy(other.gameObject);
            AddConeOnDuke();
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

}
