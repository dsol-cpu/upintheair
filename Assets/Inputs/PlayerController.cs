using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, PlayerInput.IPlayerActions
{
    private PlayerInput playerControls;
    private Vector2 movementInput;
    private bool isJumping;
    private bool isRunning;

    Vector3 moveDirection;
    float smoothTime = 0.3f;
    float hVelocity = 0f;
    float vVecolity = 0f;
    float hCurrent = 0f;
    float vCurrent = 0f;

    Transform cameraObject;
    Rigidbody rb;
    private float movementSpeed = 7f;
    private float rotationSpeed = 15;
    private Vector3 targetDirection = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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

            playerControls.Player.Jump.started+= ctx => OnJump(ctx);
            playerControls.Player.Jump.performed += ctx => OnJump(ctx);
            playerControls.Player.Jump.canceled += ctx => OnJump(ctx);

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
        moveDirection = cameraObject.forward * vCurrent + cameraObject.right * hCurrent;
        moveDirection.y = 0;
        moveDirection *= movementSpeed;
        rb.velocity = moveDirection;
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

    private void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
    }

    private void Update()
    {
        HandleAllInput();
    }
    private void FixedUpdate()
    {
        HandleAllMovement();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        isJumping = context.ReadValue<bool>();
        isJumping = context.action.triggered;
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValue<bool>();
        isRunning = context.action.triggered;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ground"))
            isJumping = false;
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
            isJumping = true;
    }


}
