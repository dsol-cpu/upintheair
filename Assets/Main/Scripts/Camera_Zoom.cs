using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class Camera_Zoom : MonoBehaviour, PlayerMovement.ICameraActions
{
    private PlayerMovement controls;
    [SerializeField] private CinemachineFramingTransposer transposer;
    [SerializeField] private float sensitivity = 10f;
    // Start is called before the first frame update
    void Start()
    {
        transposer = GetComponent<CinemachineVirtualCamera>().GetComponent<CinemachineFramingTransposer>();
    }

    void Update()
    {

    }

    void OnEnable()
    {
        if (controls == null)
        {
            controls = new PlayerMovement();
            controls.Camera.Zoom_Camera.performed += ctx => OnZoom_Camera(ctx);
        }
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Camera.Zoom_Camera.performed -= ctx => OnZoom_Camera(ctx);
        controls.Player.Disable();
    }

    public void OnLook(InputAction.CallbackContext context) { }

    public void OnZoom_Camera(InputAction.CallbackContext context)
    {
        print("scrolling");
        transposer.m_CameraDistance += context.ReadValue<float>() * sensitivity;
    }
}
