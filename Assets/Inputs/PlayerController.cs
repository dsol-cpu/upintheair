using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed = .015f;
    [SerializeField] private float jumpHeight = 0f;
    [SerializeField] private float gravityValue = -9.81f;   
//    private CharacterController controller;
    private PlayerInput playerControls;
    Vector3 movementInput = Vector3.zero;
    Vector3 playerVelocity;
    bool groundedPlayer;
    bool jumped = false;
    bool isRunning = false;

    void Awake()
    {
        playerControls = new PlayerInput();
        //controller = gameObject.GetComponent<CharacterController>();
        playerControls.Player.Move.performed += ctx => OnMove(ctx);
        playerControls.Player.Move.canceled += ctx => movementInput = Vector3.zero;
        playerControls.Player.Jump.performed += ctx => OnJump(ctx);
        playerControls.Player.Jump.canceled += ctx => jumpHeight = 0f;
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    void FixedUpdate()
    {
        transform.Translate(movementInput * Time.deltaTime);

        //could probably just make it a coordinate space and multiply speed with (0,1), (1,0), etc. and then cap speed

        if(Input.GetKey("w"))
        {
            movementInput.z += moveSpeed;
        }

        if (Input.GetKey("a"))
        {
            movementInput.x -= moveSpeed;
        }

        if (Input.GetKey("s"))
        {
            movementInput.z -= moveSpeed;
        }

        if (Input.GetKey("d"))
        {
            movementInput.x += moveSpeed;
        }
        
        if(Input.GetKeyDown("shift"))
        {
            isRunning = true;
        }

        if (Input.GetKeyDown("shift"))
        {
            isRunning = false;
        }

        if (!isRunning)
        {
            switch (movementInput) //yikes
            {
                case Vector3 v when v.x > 5f:
                    movementInput.x = 5f;
                    break;
                case Vector3 v when v.x < -5f:
                    movementInput.x = -5f;
                    break;
                case Vector3 v when v.z > 5f:
                    movementInput.z = 5f;
                    break;
                case Vector3 v when v.z < -5f:
                    movementInput.z = -5f;
                    break;

                case Vector3 v when v.x > 5f && v.z > 5f:
                    movementInput.x = 5f;
                    movementInput.z = 5f;
                    break;
                case Vector3 v when v.x < -5f && v.z < -5f:
                    movementInput.x = -5f;
                    movementInput.z = -5f;
                    break;
                case Vector3 v when v.x > 5f && v.z < -5f:
                    movementInput.x = 5f;
                    movementInput.z = -5f;
                    break;
                case Vector3 v when v.x < -5f && v.z > 5f:
                    movementInput.x = -5f;
                    movementInput.z = 5f;
                    break;
            }
        }
        if (isRunning)
        {
            switch (movementInput) //yikes
            {
                case Vector3 v when v.x > 7f:
                    movementInput.x = 7f;
                    break;
                case Vector3 v when v.x < -7f:
                    movementInput.x = -7f;
                    break;
                case Vector3 v when v.z > 7f:
                    movementInput.z = 7f;
                    break;
                case Vector3 v when v.z < -7f:
                    movementInput.z = -7f;
                    break;

                case Vector3 v when v.x > 7f && v.z > 7f:
                    movementInput.x = 7f;
                    movementInput.z = 7f;
                    break;
                case Vector3 v when v.x < -7f && v.z < -7f:
                    movementInput.x = -7f;
                    movementInput.z = -7f;
                    break;
                case Vector3 v when v.x > 7f && v.z < -7f:
                    movementInput.x = 7f;
                    movementInput.z = -7f;
                    break;
                case Vector3 v when v.x < -7f && v.z > 7f:
                    movementInput.x = -7f;
                    movementInput.z = 7f;
                    break;
            }
        }

        //controller.Move(playerVelocity * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput.x = context.ReadValue<Vector2>().normalized.x;
        movementInput.z = context.ReadValue<Vector2>().normalized.y;

        Debug.Log("i LIVE!");
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        Debug.Log("Yay fire pew pew");
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jumped = context.ReadValue<bool>();
        jumped = context.action.triggered;
    }
    public void OnTriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();

    }
    public void OnTriggerExit(Collider other)
    {
        throw new System.NotImplementedException();

    }


}
