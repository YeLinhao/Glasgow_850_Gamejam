using UnityEngine;

public class InteractionZone : MonoBehaviour
{
    public IPickupable currentPickupable;

    private void OnTriggerEnter(Collider other)
    {
        // Check if object implements IPickupable
        if (other.TryGetComponent(out IPickupable pickupable))
        {
            currentPickupable = pickupable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IPickupable pickupable))
        {
            if (pickupable == currentPickupable)
                currentPickupable = null;
        }
    }
}
