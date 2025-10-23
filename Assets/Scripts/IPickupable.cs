using UnityEngine;

public interface IPickupable
{
    void OnPickup(Transform holdParent);
    void OnDrop(Vector3 throwForce);
}
