using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, PlayerInput.IPlayerActions
{
	public float smoothTime = 1f;
	private Vector3 velocity = Vector3.zero;
	private PlayerInput playerControls;
	// Movement for X and Y
	private Vector2 movementInput;
	public Vector2 cameraInput;
	public Vector2 cameraScrollValue;

	// Where the player should go based on player input, and the angle at which the player moves
	Vector3 moveDirection;
	Transform cameraObject;
	Rigidbody rb;

	[Header("DialogueManager")]
	public Transform ChatBackGround;
	private Transform NPCCharacter;

	public DialogueManager dialogueManager;

	public float moveAmount;
	float hVelocity = 0f;
	float vVelocity = 0f;
	float hCurrent = 0f;
	float vCurrent = 0f;

	[Header("Actions")]
	public bool isInteracting;

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
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		rb = GetComponent<Rigidbody>();
		//cameraManager = FindObjectOfType<CameraManager>();
		cameraObject = Camera.main.transform;
		dialogueManager = GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>();
	}

	private void OnEnable()
	{
		if (playerControls == null)
		{
			playerControls = new PlayerInput();
			playerControls.Player.Move.performed += ctx => OnMove(ctx);

			playerControls.Player.Jump.performed += ctx => isJumping = true;

			playerControls.Player.Look.performed += ctx => OnLook(ctx);

			playerControls.Player.Zoom_Camera.performed += ctx => OnZoom_Camera(ctx);

			playerControls.Player.Sprint.performed += ctx => OnSprint(ctx);
			playerControls.Player.Sprint.canceled += ctx => OnSprint(ctx);

			playerControls.Player.Interact.performed+= ctx => OnInteract(ctx);
			playerControls.Player.Interact.canceled += ctx => OnInteract(ctx);

			playerControls.Player.Exit.performed += ctx => OnExit(ctx);
			playerControls.Player.Exit.canceled += ctx => OnExit(ctx);

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
	}

	//Handle functions are self-explanatory

	private void HandleMovementInput()
	{
		hCurrent = movementInput.x;
		vCurrent = movementInput.y;

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
		if (isJumping)
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
		Vector3 movement = new Vector3(hCurrent, 0, vCurrent) * Time.deltaTime;

		movement = cameraObject.TransformDirection(movement);
		movement.y = 0;
		movement.Normalize();

		if (isSprinting)
		{
			movement *= sprintingSpeed;
		}
		else
		{
			if (moveAmount >= 0.5f)
			{
				movement *= runningSpeed;
			}
			else
			{
				movement *= walkingSpeed;
			}
		}

		rb.velocity = movement;
		if(movement != Vector3.zero)
		{
			HandleRotation();
		}
		//print(movement);
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

		if (dialogueManager.dialogueActive || isJumping)
			return;

		HandleMovement();
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		movementInput = context.ReadValue<Vector2>();
	}

	public void OnLook(InputAction.CallbackContext context)
	{
		cameraInput = context.ReadValue<Vector2>();
		//cameraManager.RotateCamera(cameraInput);
	}

	public void OnFire(InputAction.CallbackContext context)
	{
		print("left mouse button clicked");
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		isJumping = context.action.triggered;
	}

	public void OnSprint(InputAction.CallbackContext context)
	{
		isSprinting = context.action.triggered;
	}
	public void OnZoom_Camera(InputAction.CallbackContext context)
	{
		cameraScrollValue = context.ReadValue<Vector2>();
	}

	public void OnInteract(InputAction.CallbackContext context)
	{
		isInteracting = true;
	}

    public void OnExit(InputAction.CallbackContext context)
    {
		isInteracting = false;
		print("please register yay");
    }

    public void OnTriggerEnter(Collider other)
    {
		//&& dialogueManager.dialogueInput
		if(other.tag == "NPC")
			dialogueManager.EnterRangeOfNPC();
	}

    public void OnTriggerStay(Collider other)
    {
		if (other.gameObject.tag == "NPC" && isInteracting)
		{
			print(isInteracting);
			string[] sample_string_arr = { "I am speaking wow", "wow i am so amazed yippee", "lorem ipsum and other dumb jargon" };
			dialogueManager.Names = other.name;
			dialogueManager.dialogueLines = sample_string_arr;
			dialogueManager.TriggerDialogue();
		}
		else if(!isInteracting)
        {
			dialogueManager.DropDialogue();
        }
	}

    public void OnTriggerExit(Collider other)
	{
		//end conversation
		if(other.tag == "NPC")
			dialogueManager.OutOfRange();
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
	}


}