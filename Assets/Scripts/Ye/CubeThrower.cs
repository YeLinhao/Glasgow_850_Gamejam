using UnityEngine;
using UnityEngine.InputSystem;
public class CubeThrower : MonoBehaviour
{


    [Header("投掷设置")]
    public GameObject objectToThrow;    // 要投掷的物体
    public Transform throwPoint;        // 投掷起点
    public Vector3 throwDirection = Vector3.forward; // 投掷方向
    public float throwForce = 10f;      // 投掷力度

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Throw();
        }
    }

    void Throw()
    {
        // 实例化要投掷的物体
        GameObject thrownObject = Instantiate(objectToThrow, throwPoint.position, throwPoint.rotation);

        // 获取刚体组件
        Rigidbody rb = thrownObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // 应用投掷力
            Vector3 force = throwPoint.TransformDirection(throwDirection.normalized) * throwForce;
            rb.AddForce(force, ForceMode.Impulse);
        }
    }

}
