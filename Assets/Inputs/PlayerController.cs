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
    public Vector2 cameraScrollValue;

    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody rb;

    public float moveAmount;
    float hVelocity = 0f;
    float vVecolity = 0f;
    float hCurrent = 0f;
    float vCurrent = 0f;

    [Header("Actions")]
    private bool isInteracting;

    [Header("Animator")]
    Animator animator;
    int horizontal;
    int vertical;

    [Header("Falling")]
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffset = 0.5f;
    public LayerMask groundLayer;

    [Header("Movement Flags")]
    public bool isJumping;
    public bool isSprinting;
    public bool isGrounded;

    [Header("Movement Speeds")]
    public float walkingSpeed = 1.5f;
    public float runningSpeed = 5f;
    public float sprintingSpeed = 7f;
    private float rotationSpeed = 15f;

    [Header("Jump Speeds")]
    public float jumpHeight = 6f;
    public float gravityIntensity = -15f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cameraManager = FindObjectOfType<CameraManager>();
        cameraObject = Camera.main.transform;

        //animator = GetComponent<Animator>();
        //horizontal = Animator.StringToHash("Horizontal");
        //vertical = Animator.StringToHash("Vertical");
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerInput();
            playerControls.Player.Move.started += ctx => OnMove(ctx);
            playerControls.Player.Move.performed += ctx => OnMove(ctx);
            playerControls.Player.Move.canceled += ctx => OnMove(ctx);

            playerControls.Player.Jump.performed += ctx => isJumping = true;

            playerControls.Player.Look.performed += ctx => OnLook(ctx);
            playerControls.Player.Zoom_Camera.performed += ctx => OnZoom_Camera(ctx);

            playerControls.Player.Sprint.performed += ctx => OnSprint(ctx);
            playerControls.Player.Sprint.canceled += ctx => OnSprint(ctx);
        }
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }

    private void PlayTargetAnimation(string targetAnimation, bool interaction)
    {
        //animator.SetBool("isInteracting", interaction);
        //animator.CrossFade(targetAnimation, 0.2f);
    }

    private void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        //Animation Snapping
        float snappedHorizontal;
        float snappedVertical;

        #region Snapped Horizontal
        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
            snappedHorizontal = 0.5f;
        else if (horizontalMovement > 0.55f)
            snappedHorizontal = 1f;
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            snappedHorizontal = -0.5f;
        else if (horizontalMovement < -0.55f)
            snappedHorizontal = -1f;
        else
            snappedHorizontal = 0;
        #endregion
        #region Snapped Vertical
        if (verticalMovement > 0 && verticalMovement < 0.55f)
            snappedVertical = 0.5f;
        else if (verticalMovement > 0.55f)
            snappedVertical = 1f;
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
            snappedVertical = -0.5f;
        else if (verticalMovement < -0.55f)
            snappedVertical = -1f;
        else
            snappedVertical = 0;
        #endregion


        if (isSprinting)
        {
            snappedHorizontal = horizontalMovement;
            snappedVertical = 2f;
        }

        //animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        //animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }

    private void HandleMovementInput()
    {
        float newH = Mathf.SmoothDamp(hCurrent, movementInput.x, ref hVelocity, smoothTime);
        float newV = Mathf.SmoothDamp(vCurrent, movementInput.y, ref vVecolity, smoothTime);
        hCurrent = newH;
        vCurrent = newV;

        moveAmount = Mathf.Clamp01(Mathf.Abs(hCurrent) + Mathf.Abs(vCurrent));
        UpdateAnimatorValues(0, moveAmount, isSprinting);
    }

    private void HandleSprintingInput()
    {
        if (isSprinting && moveAmount > 0.5f)
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
    }

    private void HandleJumpingInput()
    {
       if(isJumping)
        {
            HandleJumping();
            isJumping = !isJumping;
        }
    }

    private void HandleAllInput()
    {
        HandleMovementInput();
        HandleSprintingInput();
        HandleJumpingInput();
    }

    private void HandleMovement()
    {

        moveDirection.x = cameraObject.forward.x * vCurrent + cameraObject.right.x * hCurrent;
        moveDirection.y = 0;
        moveDirection.z = cameraObject.forward.z * vCurrent + cameraObject.right.z * hCurrent;

        if (isSprinting)
        {
            moveDirection *= sprintingSpeed;
        }
        else
        {
            if (moveAmount >= 0.5f)
            {
                moveDirection *= runningSpeed;
            }
            else
            {
                moveDirection *= walkingSpeed;
            }
        }

        rb.velocity = moveDirection;

    }

    private void HandleJumping()
    {
        if (isGrounded)
        {
            //animator.SetBool("isJumping", true);
            //PlayTargetAnimation("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            rb.velocity = playerVelocity;
        }
    }

    private void HandleRotation()
    {

        Vector3 targetDirection = cameraObject.forward * vCurrent + cameraObject.right * hCurrent;
        targetDirection.Normalize();
        targetDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y += rayCastHeightOffset;

        if (!isGrounded && !isJumping)
        {
            if (!isInteracting)
            {
                //PlayTargetAnimation("Falling", true);
            }
            inAirTimer += Time.deltaTime;
            rb.AddForce(transform.forward * leapingVelocity);
            rb.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }

        if (Physics.SphereCast(rayCastOrigin, 0.05f, -Vector3.up, out _, groundLayer))
        {
            if (!isGrounded && !isInteracting)
            {
                //PlayTargetAnimation("Land", true);
            }
            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void HandleAllMovement()
    {
        HandleFallingAndLanding();

        if (isInteracting || isJumping)
            return;

        HandleMovement();
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
        isJumping = context.action.triggered;
        Debug.Log(isJumping);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.action.triggered;
    }
    public void OnZoom_Camera(InputAction.CallbackContext context)
    {
        cameraScrollValue = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        HandleAllMovement();
    }

    private void FixedUpdate()
    {
        HandleAllInput();
    }

    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement(cameraInput, cameraScrollValue);

        //isInteracting = animator.GetBool("isInteracting");
        //isJumping = animator.GetBool("isJumping");
        //animator.SetBool("isGrounded", isGrounded);
    }

}