using System;
using System.Collections;
using Mono.Cecil.Cil;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using UnityEngine.UI;

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
    [SerializeField] private float speedReduction = 4f;

    [Header("Throw Arc Rendering Settings")]
    [SerializeField] private LineRenderer trajectoryLine;
    [SerializeField] private int trajectoryResolution = 30; // how smooth the arc is
    [SerializeField] private float simulationStep = 0.05f; // smaller = smoother but more expensive
    [SerializeField] private float simulationDuration = 2f; // seconds to simulate
    [SerializeField] private LayerMask collisionMask; // stops the line when hitting walls

    private float throwStartTime;
    private bool throwStarted;

   
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

    private Slider progressSlider;
    private float sliderProgress;



    void Awake()
    {
        controller = GetComponent<CharacterController>();
        defaultSpeed = speed;
    }
    private void Start()
    {
        if (trajectoryLine != null)
            trajectoryLine.enabled = false;
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
            if (!context.started && !context.canceled)
                return;

            if (heldObject == null)
            {
                if (interactionZone.currentPickupable != null)
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
            else
            {
                // When player STARTS holding the throw button
                if (context.started)
                {
                    throwStartTime = Time.time;
                    speed /= speedReduction; // slow movement while charging
                    throwStarted = true;

                }

                // When player RELEASES the throw button
                else if (context.canceled && throwStarted == true)
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
                    throwStarted = false;
                    if (trajectoryLine != null)
                        trajectoryLine.enabled = false;

                }
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
                UIManager.Instance.GameStartProgress360.gameObject.SetActive(true);
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
                sliderProgress = 0;
                UIManager.Instance.GameStartProgress360.value = sliderProgress;
                // Hide 360 progress bar
                UIManager.Instance.GameStartProgress360.gameObject.SetActive(false);

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
    private void DrawThrowArc()
    {
        if (throwStarted && heldObject != null)
        {
            float holdTime = Time.time - throwStartTime;
            float t = Mathf.Clamp01(holdTime / maxChargeTime);
            float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, t);

            Vector3 throwDirection = Quaternion.AngleAxis(throwAngle, transform.right) * transform.forward;
            Vector3 force = throwDirection * throwForce;

            ShowTrajectory(holdPoint.position, force / (heldObject.gameObject.GetComponent<Rigidbody>()?.mass ?? 1f));

        }
        else if (trajectoryLine.enabled)
        {
            trajectoryLine.enabled = false;
        }
    }

    private void ShowTrajectory(Vector3 startPos, Vector3 initialVelocity)
    {
        if (trajectoryLine == null)
            return;

        trajectoryLine.enabled = true;

        Vector3[] points = new Vector3[trajectoryResolution];
        Vector3 currentPosition = startPos;
        Vector3 velocity = initialVelocity;
        float step = simulationDuration / trajectoryResolution;

        for (int i = 0; i < trajectoryResolution; i++)
        {
            points[i] = currentPosition;
            // Simple physics: p = p0 + v*t + 0.5*g*t?
            velocity += Physics.gravity * step;
            currentPosition += velocity * step;

            // Optional: stop if raycast hits something
            if (Physics.Raycast(points[i], velocity.normalized, out RaycastHit hit, velocity.magnitude * step, collisionMask))
            {
                points[i + 1 >= trajectoryResolution ? trajectoryResolution - 1 : i + 1] = hit.point;
                trajectoryLine.positionCount = i + 2;
                trajectoryLine.SetPositions(points);
                return;
            }
        }

        trajectoryLine.positionCount = trajectoryResolution;
        trajectoryLine.SetPositions(points);
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
            DrawThrowArc();

        }
        else if (GameManager.CurrentState == GameManager.GameState.PreGame && interactIsHoldingStart)
        {
            sliderProgress = Mathf.Clamp01((Time.time - interactHoldStartTime) / holdDurationToStart);
            UIManager.Instance.GameStartProgress360.value = sliderProgress;
            if (sliderProgress >= 1f)
            {
                // Complete interaction
                interactIsHoldingStart = false;
                UIManager.Instance.GameStartProgress360.gameObject.SetActive(false);
                GameManager.instance.StartGame();
            }

        }
    }
}
