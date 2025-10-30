using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.GraphicsBuffer;

public class DropShadowScaler : MonoBehaviour
{
    [SerializeField] private DecalProjector shadow;
    [SerializeField] private Transform target; 
    [SerializeField] private float heightOffset = 0.05f; // how far below the cone base
    private float baseSize = 1f;

    void Update()
    {
        float height = transform.position.y;
        float scale = Mathf.Lerp(baseSize, baseSize * 0.4f, height / 5f);
        shadow.size = new Vector3(scale,scale, shadow.size.z);
    }
    private void LateUpdate()
    {
        if (target == null) return;

        // Follow the cone’s X/Z position, but stay at a fixed height under it
        Vector3 newPos = target.position;
        newPos.y = target.position.y - heightOffset;
        shadow.transform.position = newPos;

        //// Keep it always flat
        shadow.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
