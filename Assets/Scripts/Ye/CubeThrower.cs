using UnityEngine;
using UnityEngine.InputSystem;
public class CubeThrower : MonoBehaviour
{


    [Header("Ͷ������")]
    public GameObject objectToThrow;    // ҪͶ��������
    public Transform throwPoint;        // Ͷ�����
    public Vector3 throwDirection = Vector3.forward; // Ͷ������
    public float throwForce = 10f;      // Ͷ������

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Throw();
        }
    }

    void Throw()
    {
        // ʵ����ҪͶ��������
        GameObject thrownObject = Instantiate(objectToThrow, throwPoint.position, throwPoint.rotation);

        // ��ȡ�������
        Rigidbody rb = thrownObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Ӧ��Ͷ����
            Vector3 force = throwPoint.TransformDirection(throwDirection.normalized) * throwForce;
            rb.AddForce(force, ForceMode.Impulse);
        }
    }

}
