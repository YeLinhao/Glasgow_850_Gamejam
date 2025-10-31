using UnityEngine;

public class PickupableItem : MonoBehaviour, IPickupable
{
    private Rigidbody rb;
    private Collider col;
    public CharacterController owner;
    [SerializeField] private GameObject impactParticlesPrefab;

    [Header("Motion Trail Settings")]
    [SerializeField] private TrailRenderer trail; // assign in inspector
    [SerializeField] private float speedThreshold = 5f; // speed to start fading
    [SerializeField] private float maxSpeed = 20f;      // speed at which trail is at maximum length/width
    [SerializeField] private float fadeSpeed = 2f;      // how fast trail length/width interpolates
    [SerializeField] private float maxTrailWidth = 0.5f; // maximum trail width
    private float defaultTrailTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (trail != null)
        {
            defaultTrailTime = trail.time;
            trail.time = 0f; // start invisible
            trail.widthMultiplier = 0f; // start thin
        }
    }

    private void Update()
    {
        if (trail == null || rb == null) return;

        float speed = rb.linearVelocity.magnitude;

        // Determine target trail length based on speed
        float normalizedSpeed = Mathf.Clamp01((speed - speedThreshold) / (maxSpeed - speedThreshold));
        float targetTrailTime = normalizedSpeed * defaultTrailTime;
        float targetTrailWidth = normalizedSpeed * maxTrailWidth;

        // Smoothly interpolate trail properties
        trail.time = Mathf.Lerp(trail.time, targetTrailTime, Time.deltaTime * fadeSpeed);
        trail.widthMultiplier = Mathf.Lerp(trail.widthMultiplier, targetTrailWidth, Time.deltaTime * fadeSpeed);
    }

    public void OnPickup(Transform holdParent, CharacterController player)
    {
        rb.isKinematic = true;
        col.enabled = false;

        transform.SetParent(holdParent);
        owner = player;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Debug.Log($"Picked up {name}");
    }

    public void OnDrop(Vector3 throwForce)
    {
        transform.SetParent(null);
        rb.isKinematic = false;
        col.enabled = true;
        rb.AddForce(throwForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (impactParticlesPrefab != null && collision.contacts.Length > 0)
            {
                ContactPoint contact = collision.contacts[0];
                GameObject particles = Instantiate(impactParticlesPrefab, contact.point, Quaternion.LookRotation(contact.normal));
                Destroy(particles, 2f);
            }
        }
    }
}
