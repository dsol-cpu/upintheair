using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerControls;
    private Vector2 movementInput;
    public float verticalAxis;
    public float horizontalAxis;

    Vector3 moveDirection;
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
            playerControls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
            playerControls.Player.Move.canceled += ctx => movementInput = Vector2.zero;
        }
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void HandleMovementInput()
    {
        verticalAxis = movementInput.y;
        horizontalAxis = movementInput.x;
    }

    private void HandleAllInput()
    {
        HandleMovementInput();
    }

    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * verticalAxis + cameraObject.right * horizontalAxis;
        moveDirection.Normalize();
        moveDirection.y = 0;
        moveDirection *= movementSpeed;
        rb.velocity = moveDirection;
    }

    private void HandleRotation()
    {
        targetDirection = cameraObject.forward * verticalAxis + cameraObject.right * horizontalAxis;
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

}
