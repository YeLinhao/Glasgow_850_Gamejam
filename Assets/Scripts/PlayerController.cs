using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float turnSpeed = 360;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;
    private Vector3 move;

    private bool holdingObject = true;
    private bool canDash;
    private bool isDashing;

    public float dashForce;
    public float dashDuration;
    
    

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (holdingObject)
        {
            if (context.performed)
            {
                speed = speed / 4;
                // Collect start time
            }
            if (context.action.WasReleasedThisFrame())
            {
                speed = speed * 4;
                // if object is throwable then throw here with power = duration held
                
            }
        }
        else
        {
            //Check for object in front arc, if object is grabbable then grab object
        }
        
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("X pressed");
            canDash = false;
            isDashing = true;
            rb.MovePosition(transform.position + (transform.forward * move.normalized.magnitude * dashForce) * speed * Time.deltaTime);

        }
    }


    private void Look()
    {
        
        if (move == Vector3.zero) return;
        var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        var skewedInput = matrix.MultiplyPoint3x4(move);

        var relative = (transform.position + skewedInput) - transform.position;
        var rot = Quaternion.LookRotation(relative, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        move = new Vector3(moveInput.x, 0, moveInput.y);
        Look();
        rb.MovePosition(transform.position + (transform.forward * move.normalized.magnitude) * speed * Time.deltaTime);
    }
}
