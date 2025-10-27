using Mono.Cecil.Cil;
using System.Collections;
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
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float turnSpeed = 360;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Throw Settings")]
    [SerializeField] private float minThrowForce = 10f;
    [SerializeField] private float maxThrowForce = 30f;
    [SerializeField] private float maxChargeTime = 2f;
    [SerializeField] private float throwAngle = 30f; // degrees
    private float throwStartTime;

   
    [SerializeField] private float holdDurationToStart = 2f;
    private float interactHoldStartTime;
    private bool interactIsHoldingStart = false;


    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 move;
    private float defaultSpeed;

    private bool isDashing = false;
    private bool canDash = true;

    private float dashTimer;
    private float dashCooldownTimer;

    private IPickupable heldObject;



    void Awake()
    {
        controller = GetComponent<CharacterController>();
        defaultSpeed = speed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (GameManager.CurrentState == GameManager.GameState.MainGame)
        {
            moveInput = context.ReadValue<Vector2>();
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (GameManager.CurrentState == GameManager.GameState.MainGame)
        {
            if (heldObject != null)
            {
                // When player STARTS holding the throw button
                if (context.started || context.performed)
                {
                    throwStartTime = Time.time;
                    speed /= 4f; // slow movement while charging
                }

                // When player RELEASES the throw button
                if (context.canceled)
                {
                    speed = defaultSpeed;

                    float holdTime = Time.time - throwStartTime;
                    float t = Mathf.Clamp01(holdTime / maxChargeTime);
                    float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, t);

                    // Compute a throw direction at an upward angle
                    Vector3 throwDirection = Quaternion.AngleAxis(throwAngle, transform.right) * transform.forward;

                    Vector3 force = throwDirection * throwForce;
                    (heldObject as PickupableItem)?.OnDrop(force);
                    heldObject = null;

                }
            }
            else if (interactionZone.currentPickupable != null)
            {
                // Pick up nearby object
                heldObject = interactionZone.currentPickupable;
                heldObject.OnPickup(holdPoint, controller);
                interactionZone.currentPickupable = null;
            }
            else
            {
                Debug.Log("No pickupable object in range");
            }

        }

        // PREGAME COMMANDS
        else if (GameManager.CurrentState == GameManager.GameState.PreGame)
        {
            // When player starts holding the button
            if (context.started)
            {
                interactIsHoldingStart = true;
                interactHoldStartTime = Time.time;
                // Start progress bar animation
            }

            // When player releases the button
            if (context.canceled)
            {
                interactIsHoldingStart = false;
                float totalHoldTime = Time.time - interactHoldStartTime;

                if (totalHoldTime >= holdDurationToStart)
                {

                    GameManager.CurrentState = GameManager.GameState.MainGame;
                }
                else
                {

                    // Cancel progress animation
                }
            }
            

        }


    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (GameManager.CurrentState == GameManager.GameState.MainGame)
        {
            if (context.performed && canDash && !isDashing)
            {
                Debug.Log("X pressed");
                StartCoroutine(DashRoutine());
            }
        }
        // PREGAME COMMANDS
        else if (GameManager.CurrentState == GameManager.GameState.PreGame)
        {
            if (context.performed)
            {
                PlayerInput playerInput = controller.GetComponentInParent<PlayerInput>();
                PlayerInputManager.instance.RemovePlayer(playerInput);
            }
   

        }
    }

    private IEnumerator DashRoutine()
    {
        if (GameManager.CurrentState == GameManager.GameState.MainGame)
        {


            isDashing = true;
            canDash = false;

            float startTime = Time.time;
            Vector3 dashDirection = transform.forward;

            // Optional: disable gravity/movement control during dash
            while (Time.time < startTime + dashDuration)
            {
                controller.Move(dashDirection * dashForce * Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            }

            isDashing = false;

            // Start cooldown
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
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
// rb.AddForce(-rb.GetAccumulatedForce());
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (GameManager.CurrentState == GameManager.GameState.MainGame)
        {


            if (isDashing)
                return; // Skip normal movement while dashing

            move = new Vector3(moveInput.x, 0, moveInput.y);
            Look();
            Vector3 moveDirection = transform.forward * move.magnitude * speed * Time.deltaTime;
            controller.Move(moveDirection);
            controller.Move(Vector3.up * gravity * Time.deltaTime);
        }
        else if (GameManager.CurrentState == GameManager.GameState.PreGame)
        {
            if (interactIsHoldingStart && Time.time - interactHoldStartTime >= holdDurationToStart)
            {
                interactIsHoldingStart = false; // stop multiple triggers
                GameManager.instance.StartGame();
                Debug.Log("Hold complete — starting the game!");
            }

        }
    }
}
