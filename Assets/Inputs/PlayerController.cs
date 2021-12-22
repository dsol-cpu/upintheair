using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, PlayerInput.IPlayerActions
{
    private CameraManager cameraManager;
    public float smoothTime = 1f;
    private Vector3 velocity = Vector3.zero;
    private PlayerInput playerControls;
    private Vector2 movementInput;
    public Vector2 cameraInput;
    private Vector3 targetDirection = Vector3.zero;

    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody rb;

    float hVelocity = 0f;
    float vVecolity = 0f;
    float hCurrent = 0f;
    float vCurrent = 0f;

    [Header("Falling")]
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffset = 0.5f;
    public LayerMask groundLayer;

    [Header("Movement Flags")]
    private bool isJumping;
    private bool isRunning;
    private bool isGrounded;

    [Header("Movement Speeds")]
    public float walkingSpeed = 1.5f;
    public float runningSpeed = 5f;
    private float movementSpeed = 7f;
    private float rotationSpeed = 15f;

    [Header("Jump Speeds")]
    public float jumpHeight = 3f;
    public float gravityIntensity = -15f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cameraManager = FindObjectOfType<CameraManager>();
        cameraObject = Camera.main.transform;
    }

    private void OnEnable()
    {
        if(playerControls == null)
        {
            playerControls = new PlayerInput();
            playerControls.Player.Move.started += ctx => OnMove(ctx);
            playerControls.Player.Move.performed+= ctx => OnMove(ctx);
            playerControls.Player.Move.canceled += ctx => OnMove(ctx);

            playerControls.Player.Jump.started += ctx => OnJump(ctx);

            playerControls.Player.Look.performed += ctx => OnLook(ctx);
        }
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }

    private void HandleMovementInput()
    {
        float newH = Mathf.SmoothDamp(hCurrent, movementInput.x, ref hVelocity, smoothTime);
        float newV = Mathf.SmoothDamp(vCurrent, movementInput.y, ref vVecolity, smoothTime);
        hCurrent = newH;
        vCurrent = newV;
    }

    private void HandleAllInput()
    {
        HandleMovementInput();
    }

    private void HandleMovement()
    {
        moveDirection.x = cameraObject.forward.x * vCurrent + cameraObject.right.x * hCurrent;
        moveDirection.z = cameraObject.forward.z * vCurrent + cameraObject.right.z * hCurrent;
        moveDirection.x *= movementSpeed;
        moveDirection.z *= movementSpeed;
        rb.velocity = moveDirection;

       


    }
    private void HandleJumping()
    {
        float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
        Vector3 playerVelocity = moveDirection;
        playerVelocity.y = jumpingVelocity;
        rb.velocity = playerVelocity;
    }

    private void HandleRotation()
    {
        targetDirection = cameraObject.forward * vCurrent + cameraObject.right * hCurrent;
        targetDirection.Normalize();
        targetDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
    }

    private void HandleAllMovement()
    {
        HandleMovement();
        HandleJumping();
        HandleRotation();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        cameraInput = context.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        isJumping = context.ReadValue<bool>();
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValue<bool>();
        isRunning = context.action.triggered;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Debug.Log("touch-y mmmmm!");
            isGrounded = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    private void Update()
    {
        HandleAllInput();
    }
    private void FixedUpdate()
    {
        HandleAllMovement();
        cameraManager.HandleAllCameraMovement();
    }

    private void LateUpdate()
    {
    }
}