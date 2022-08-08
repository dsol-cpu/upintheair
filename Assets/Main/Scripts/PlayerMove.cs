using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Player
{
	public class PlayerMove : MonoBehaviour, PlayerMovement.IPlayerActions
	{
		public float smoothTime = 1f;
		private Vector3 velocity = Vector3.zero;
		private PlayerMovement playerControls;
		Animator animator;
		AnimatorManager animatorManager;
		DialogueInteract dialogueInteract;
		// Movement for X and Y
		private Vector2 movementInput;

		// Where the player should go based on player input, and the angle at which the player moves
		Vector3 moveDirection;
		float moveSpeed = 7f;
		Transform cameraObject;
		Rigidbody rb;
		private bool readyToJump = true;

		[Header("DialogueManager")]
		public Transform ChatBackGround;
		private Transform NPCCharacter;

		public float moveAmount;
		float hCurrent = 0f;
		float vCurrent = 0f;

		[Header("Actions")]
		public bool isInteracting;
		public bool isJumping;

		[Header("Falling")]
		public float inAirTimer;
		public float leapingVelocity;
		public float fallingVelocity;
		public float rayCastHeightOffset = 0.5f;
		public LayerMask groundLayer;

		[Header("Movement Flags")]
		public bool isSprinting;
		public bool isGrounded;

		[Header("Movement Speeds")]
		public float walkingSpeed = 1.5f;
		public float runningSpeed = 5f;
		public float sprintingSpeed = 7f;
		private float rotationSpeed = 15f;
		private float groundDrag = 5f;

		[Header("Jump Speeds")]
		public float jumpHeight = 6f;
		public float gravityIntensity = 9.8f;

		[Header("Dialogue")]
		[SerializeField] private bool didWeTalk = false;

		public bool DidWeTalk { get => didWeTalk; set => didWeTalk = value; }

		private void Awake()
		{
			dialogueInteract = GetComponent<DialogueInteract>();
			animator = GetComponent<Animator>();
			Application.targetFrameRate = 60;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			animatorManager = GetComponent<AnimatorManager>();
			rb = GetComponent<Rigidbody>();
			cameraObject = Camera.main.transform;
		}

		private void Start()
		{

		}

		private void OnEnable()
		{
			if (playerControls == null)
			{
				playerControls = new PlayerMovement();  
				playerControls.Player.Move.performed += ctx => OnMove(ctx);
				playerControls.Player.Move.canceled += ctx => OnMove(ctx);

				playerControls.Player.Jump.performed += ctx => OnJump(ctx);
				playerControls.Player.Jump.performed += ctx => OnJump(ctx);

				playerControls.Player.Sprint.performed += ctx => OnSprint(ctx);
				playerControls.Player.Sprint.canceled += ctx => OnSprint(ctx);

				playerControls.Player.Interact.performed += ctx => OnInteract(ctx);

				playerControls.Player.Exit.performed += ctx => OnExit(ctx);

			}
			playerControls.Player.Enable();
		}

		private void OnDisable()
		{
			playerControls.Player.Move.performed -= ctx => OnMove(ctx);
			playerControls.Player.Move.canceled -= ctx => OnMove(ctx);

			playerControls.Player.Jump.performed -= ctx => OnJump(ctx);
			playerControls.Player.Jump.performed -= ctx => OnJump(ctx);

			playerControls.Player.Sprint.performed -= ctx => OnSprint(ctx);
			playerControls.Player.Sprint.canceled -= ctx => OnSprint(ctx);

			playerControls.Player.Interact.performed -= ctx => OnInteract(ctx);

			playerControls.Player.Exit.performed -= ctx => OnExit(ctx);
			playerControls.Player.Disable();
		}

		//Handle functions are self-explanatory
		private void HandleMovementInput()
		{
			hCurrent = movementInput.x;
			vCurrent = movementInput.y;
			moveAmount = Mathf.Clamp01(Mathf.Abs(hCurrent) + Mathf.Abs(vCurrent));
			animatorManager.UpdateAnimatorValues(0, moveAmount, isSprinting);
		}
		private void HandleMovement()
		{
			Vector3 movement = new Vector3(hCurrent, 0, vCurrent);

			movement = cameraObject.TransformDirection(movement);
			movement.y = 0;
			movement.Normalize();

			if (isSprinting) movement *= sprintingSpeed;
			else
			{
				if (moveAmount >= 0.5f) movement *= runningSpeed;
				else movement *= walkingSpeed;
			}
			rb.AddForce(movement, ForceMode.VelocityChange);
			if (movement != Vector3.zero && !isJumping) HandleRotation();
		}

		private void SpeedControl()
		{
			Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

			// limit velocity if needed
			if (flatVel.magnitude > moveSpeed)
			{
				Vector3 limitedVel = flatVel.normalized * moveSpeed;
				if (!isGrounded)
					limitedVel /= 2f;
				rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
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
			RaycastHit hit;
			Vector3 rayCastOrigin = transform.position;
			rayCastOrigin.y += rayCastHeightOffset;
			if (!isGrounded && !isJumping)
			{
				if (!isInteracting)
				{
					animatorManager.PlayTargetAnimation("Falling", true);
				}
				inAirTimer += Time.deltaTime;
				rb.AddForce(transform.forward * leapingVelocity);
				rb.AddForce(Vector3.down * fallingVelocity * inAirTimer);
			}
			if(Physics.Raycast(rayCastOrigin, Vector3.down, out hit, .7f, groundLayer))
            {
				
				if (!isGrounded && !isInteracting)
				{
					animatorManager.PlayTargetAnimation("Landing", true);
				}
					isGrounded = true;
					readyToJump = true;
					inAirTimer = 0f;
				}
				else
				{
					isGrounded = false;
				}		
			
		}

		private void HandleAllMovement()
		{
			HandleMovementInput();
			HandleFallingAndLanding();
			HandleMovement();
			SpeedControl();

			// handle drag
			if (isGrounded)
				rb.drag = groundDrag;
			else
				rb.drag = 0;
		}

		private void Update()
		{

		}

		private void FixedUpdate()
		{
			HandleAllMovement();
		}

		private void LateUpdate()
		{
			isInteracting = animator.GetBool("isInteracting");
			isJumping = animator.GetBool("isJumping");
			animatorManager.animator.SetBool("isGrounded", isGrounded);
		}
		public void OnMove(InputAction.CallbackContext context)
		{
			movementInput = context.ReadValue<Vector2>();
			
			HandleMovement();
		}

		public void OnFire(InputAction.CallbackContext context)
		{
			print("left mouse button clicked");
		}

		public void OnJump(InputAction.CallbackContext context)
		{
			if (readyToJump && isGrounded)
			{
				rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
				animatorManager.animator.SetBool("isJumping", true);
				animatorManager.PlayTargetAnimation("Jumping", false);
			}
		}

		public void OnSprint(InputAction.CallbackContext context)
		{
			isSprinting = context.action.triggered;
		}

		public void OnInteract(InputAction.CallbackContext context)
		{
			dialogueInteract.CheckForNearbyNPC();
			print("Called interaction");
		}

		public void OnExit(InputAction.CallbackContext context)
		{
			//saveGame();
		#if UNITY_STANDALONE
			Application.Quit();
		#endif
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#endif
			Debug.Log("Quitting");
		}

	}
}