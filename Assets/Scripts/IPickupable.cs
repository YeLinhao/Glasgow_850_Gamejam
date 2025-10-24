using UnityEngine;

public interface IPickupable
{
    void OnPickup(Transform holdParent, CharacterController player);
    void OnDrop(Vector3 throwForce);
}
