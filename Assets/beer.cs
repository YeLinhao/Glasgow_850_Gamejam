using UnityEngine;
using System.Collections;
using System.Threading;

public class beer : MonoBehaviour
{
    [Header("Drunk Effect Settings")]
    public float drunkDuration = 30f;
    public float stumbleIntensity = 10f;       // degrees per second of random rotation
    public float wobbleAmount = 0.5f;          // movement deviation
    public Color drunkTint = new Color(0.9f, 0.8f, 0.4f, 1f); // yellow-brown

    [Header("References")]
    public GameObject beerModel;
    public GameObject beerIcon;
    public ParticleSystem beerBubblesPrefab;

    private bool hasTriggered = false;
    private ParticleSystem beerBubblesInstance;
    public float stumbleTime = 0.5f;             // how long to move in one random direction
    private float stumbleTimer = 0f;
    private Vector3 randomOffset = Vector3.zero;

    private Renderer renderer;

    [SerializeField] private float spinSpeed = 90f;

        
    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            
            GetComponent<AudioSource>()?.Play();

            StartCoroutine(DrunkEffectRoutine(other.gameObject));
            beerModel.SetActive(false);
        }
    }

    private IEnumerator DrunkEffectRoutine(GameObject player)
    {
        // --- Setup references ---
        var controller = player.GetComponent<CharacterController>();
        Transform pigeonBlender = player.transform.Find("pigeon blender");
        if (pigeonBlender != null)
        {
            // Find the child "pigeon body" under "pigeon blender"
            Transform pigeonBody = pigeonBlender.Find("pigeon body");
            if (pigeonBody != null)
            {
                renderer = pigeonBody.GetComponent<Renderer>();
            }
        }
        var originalColor = renderer != null ? renderer.material.color : Color.white;
        var playerController = player.GetComponent<PlayerController>(); // your movement script

        // --- Spawn bubbles effect ---
        if (beerBubblesPrefab != null)
        {
            GameObject instance = Instantiate(beerBubblesPrefab.gameObject, player.transform);
            beerBubblesInstance = instance.GetComponent<ParticleSystem>();
            instance.transform.localPosition = new Vector3(0, 2f, 0);
            beerBubblesInstance.Play();
      
        }

        // --- Tint player ---
        if (renderer != null)
            renderer.material.color = drunkTint;

        // --- Enable drunk state ---
        player.GetComponent<PlayerController>().isDrunk = true;
       

        float elapsed = 0f;
        Debug.Log("Player is drunk!");

        // --- Apply wobble + rotation over time ---
        while (elapsed < drunkDuration)
        {
            elapsed += Time.deltaTime;
            stumbleTimer -= Time.deltaTime;

            // Rotate Y wobble continuously
            player.transform.Rotate(Vector3.up, Random.Range(-stumbleIntensity, stumbleIntensity) * Time.deltaTime);

            if (controller != null && controller.enabled)
            {
                // Pick a new random direction when timer runs out
                if (stumbleTimer <= 0f)
                {
                    randomOffset = new Vector3(
                        Random.Range(-wobbleAmount, wobbleAmount),
                        0,
                        Random.Range(-wobbleAmount, wobbleAmount)
                    );
                    stumbleTimer = stumbleTime; // reset timer
                }

                // Move in the current stumble direction
                controller.Move(randomOffset * Time.deltaTime);
            }

            yield return null;
        }

        // --- Reset player ---
        if (beerBubblesInstance != null)
        {
            beerBubblesInstance.Stop();
            Destroy(beerBubblesInstance.gameObject, 2f);
        }

        if (renderer != null)
            renderer.material.color = originalColor;

        player.GetComponent<PlayerController>().isDrunk = false;

        Debug.Log("Player sobered up.");
        Destroy(gameObject);
    }
    private void Update()
    {
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
    }
}
