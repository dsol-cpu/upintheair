using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, PlayerInput.IPlayerActions
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravityValue = -9.81f;   
//    private CharacterController controller;
    private PlayerInput playerControls;
    Vector2 movementInput = Vector2.zero;
    Vector3 playerVelocity;
    bool groundedPlayer;
    bool jumped;

    void Awake()
    {
        playerControls = new PlayerInput();
        //controller = gameObject.GetComponent<CharacterController>();
        playerControls.Player.Move.performed += ctx => OnMove(ctx);
        playerControls.Player.Move.canceled += ctx => movementInput = Vector3.zero;
        playerControls.Player.Jump.performed += ctx => OnJump(ctx);
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
        transform.Translate(new Vector3(movementInput.x, jumpHeight, movementInput.y) * Time.deltaTime * moveSpeed);
        if (jumped)
            jumpHeight = 5f;
        //controller.Move(playerVelocity * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
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
