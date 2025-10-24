using System.Buffers;
using UnityEngine;

public class PickupableItem : MonoBehaviour, IPickupable
{
    private Rigidbody rb;
    private Collider col;
    public CharacterController owner;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void OnPickup(Transform holdParent, CharacterController player)
    {
        // Disable physics while being held
        rb.isKinematic = true;
        col.enabled = false;

        // Parent to hold point
        transform.SetParent(holdParent);
        owner = player;


        // Reset local position/rotation for proper alignment
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Debug.Log($"Picked up {name}");
    }

    public void OnDrop(Vector3 throwForce)
    {
        // Re-enable physics and apply a force
        transform.SetParent(null);
        rb.isKinematic = false;
        col.enabled = true;
        rb.AddForce(throwForce, ForceMode.Impulse);
    }
}
