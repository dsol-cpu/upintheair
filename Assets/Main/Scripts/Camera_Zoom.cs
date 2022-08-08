using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class Camera_Zoom : MonoBehaviour, PlayerMovement.ICameraActions
{
    private PlayerMovement controls;
    [SerializeField] private CinemachineFramingTransposer transposer;
    [SerializeField] private float sensitivity = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        transposer = GetComponent<CinemachineVirtualCamera>().GetComponentInChildren<CinemachineFramingTransposer>();
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
        controls.Camera.Enable();
    }

    void OnDisable()
    {
        controls.Camera.Zoom_Camera.performed -= ctx => OnZoom_Camera(ctx);
        controls.Camera.Disable();
    }

    public void OnLook(InputAction.CallbackContext context) { }

    public void OnZoom_Camera(InputAction.CallbackContext context)
    {
        transposer.m_CameraDistance -= Mathf.Clamp(context.ReadValue<Vector2>().y,-sensitivity, sensitivity);
    }
}
