using UnityEngine;
using System.Collections;

public class IrnBru : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


    }

    public float enlargeMultiplier = 3f;
    public float duration = 30f;
    public GameObject IrnbruModel;
    private bool hasTriggered = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && hasTriggered == false)
        {
            hasTriggered = true;
            IrnbruModel.SetActive(false);
            StartCoroutine(EnlargeTemporarily(other.transform));
        }


    }


    private IEnumerator EnlargeTemporarily(Transform player)
    {
        Vector3 originalScale = player.localScale; 
        player.localScale = originalScale * enlargeMultiplier; 
        Debug.Log("Player enlarged!");

        yield return new WaitForSeconds(duration); 


        player.localScale = originalScale; 
        Debug.Log("Player back to normal");
        Destroy(this.gameObject);
    }



}
