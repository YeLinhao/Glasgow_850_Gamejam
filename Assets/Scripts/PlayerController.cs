using Mono.Cecil.Cil;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InteractionZone interactionZone;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private Rigidbody rb;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float turnSpeed = 360;


    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;
    private Vector3 move;

    private bool holdingObject = false;
    private bool canDash;
    private bool isDashing;

    public float dashForce;
    public float dashDuration;

    private IPickupable heldObject;



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
        if (heldObject != null)
        {
            if (context.performed)
            {
                speed = speed / 4;
                // TODO Collect start time
            }
            if (context.action.WasReleasedThisFrame())
            {
                speed = speed * 4;
                // TODO throw with power = duration held
                Vector3 force = transform.forward * 5f;
                (heldObject as PickupableItem)?.OnDrop(force);
                heldObject = null;

            }
        }
        else if (interactionZone.currentPickupable != null)
        {
            // Pick up nearby object
            heldObject = interactionZone.currentPickupable;
            heldObject.OnPickup(holdPoint);
            interactionZone.currentPickupable = null;
        }
        else
        {
            Debug.Log("No pickupable object in range");
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
