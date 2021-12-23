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

    public float cameraInputX;
    public float cameraInputY;

    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody rb;

    public float moveAmount;
    float hVelocity = 0f;
    float vVecolity = 0f;
    float hCurrent = 0f;
    float vCurrent = 0f;

    public bool b_Input;

    [Header("")]
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

        animator = GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerInput();
            playerControls.Player.Move.started += ctx => OnMove(ctx);
            playerControls.Player.Move.performed += ctx => OnMove(ctx);
            playerControls.Player.Move.canceled += ctx => OnMove(ctx);

            playerControls.Player.Jump.performed += ctx => OnJump(ctx);

            playerControls.Player.Look.performed += ctx => OnLook(ctx);

            playerControls.Player.Run.started += ctx => b_Input = true;
            playerControls.Player.Run.performed += ctx => b_Input = true;
            playerControls.Player.Run.performed += ctx => b_Input = false;

        }
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }

    private void PlayTargetAnimation(string targetAnimation, bool isInteracting)
    {
        animator.SetBool("isInteracting", isInteracting);
        animator.CrossFade(targetAnimation, 0.2f);
    }

    private void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isRunning)
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



        if (isRunning)
        {
            snappedHorizontal = horizontalMovement;
            snappedVertical = 22f;
        }


        animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }

    private void HandleMovementInput()
    {
        float newH = Mathf.SmoothDamp(hCurrent, movementInput.x, ref hVelocity, smoothTime);
        float newV = Mathf.SmoothDamp(vCurrent, movementInput.y, ref vVecolity, smoothTime);
        hCurrent = newH;
        vCurrent = newV;

        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;

        moveAmount = Mathf.Clamp01(Mathf.Abs(hCurrent) + Mathf.Abs(vCurrent));
        UpdateAnimatorValues(0, moveAmount, isRunning);
    }

    private void HandleRunningInput()
    {
        if (b_Input && moveAmount > 0.5f)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }

    private void HandleJumpingInput()
    {

    }

    private void HandleAllInput()
    {
        HandleMovementInput();
        HandleRunningInput();
        //HandleJumpingInput();
    }

    private void HandleMovement()
    {
        if (isJumping)
            return;

        moveDirection.x = cameraObject.forward.x * vCurrent + cameraObject.right.x * hCurrent;
        moveDirection.z = cameraObject.forward.z * vCurrent + cameraObject.right.z * hCurrent;
        moveDirection.x *= movementSpeed;
        moveDirection.z *= movementSpeed;
        rb.velocity = moveDirection;


        if (isRunning)
        {
            moveDirection *= runningSpeed;
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

    }
    private void HandleJumping()
    {
        if (isGrounded)
        {
            animator.SetBool("isJumping", true);
            PlayTargetAnimation("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            rb.velocity = playerVelocity;
        }

    }

    private void HandleRotation()
    {
        if (isJumping)
            return;

        Vector3 targetDirection = cameraObject.forward * vCurrent + cameraObject.right * hCurrent;
        targetDirection.Normalize();
        targetDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        //rayCastOrigin.y += rayCastHeightOffSet;

        if (!isGrounded && !isJumping)
        {
            if (!isInteracting)
            {
                PlayTargetAnimation("Falling", true);
            }
            inAirTimer += Time.deltaTime;
            rb.AddForce(transform.forward * leapingVelocity);
            rb.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }
        if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, groundLayer))
        {
            if (!isGrounded && !isInteracting)
            {
                PlayTargetAnimation("Land", true);
            }
        }
    }

    private void HandleAllMovement()
    {
        //HandleFallingAndLanding();

        if (isInteracting)
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

    }
    public void OnRun(InputAction.CallbackContext context)
    {
        b_Input = context.ReadValue<bool>();
        b_Input = context.action.triggered;
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
        animator.GetBool("isInteracting");
        isJumping = animator.GetBool("isJumping");
    }

}