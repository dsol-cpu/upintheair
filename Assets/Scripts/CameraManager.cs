using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Target and Position Settings")]
    public Transform targetTransform; // The object the camera will follow
    public Transform cameraPivot; // The object the camera uses to pivot (Look up and down)
    public Transform cameraTransform; //The transform of the actual camera in the scene
    public LayerMask collisionLayers; // The layers we want our camera to collide with
    private float defaultPosition; //set to local 'Z' position of the camera

    [Header("Camera Speed and Behavior Settings")]
    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPosition;
    public float cameraCollisionOffset = 0.2f; // The value at which the camera will repel objects
    public float minimumCollisionOffset = 0.2f;
    public float cameraCollisionRadius = 2f;
    public float cameraFollowSpeed = 0.05f; // How fast the camera follows the target
    public float cameraHorizSpeed = .5f; // How fast the camera will be in looking left and right
    public float cameraVertSpeed = .5f; // How fast the camera will be in looking up and down
    
    [Header("Camera Mouse Settings")]
    public float vertAxisAngle; // The current vertical angle of the camera
    public float horizAxisAngle; // The current horizontal angle of the camera
    public float minHorizAxisAngle = -90; // Minimum angle for the camera pivot
    public float maxHorizAxisAngle = 90; // Maximum angle for the camera pivot
    
    [Header("Camera Zoom Settings")]
    private float zoomDistance;
    private float minZoomDistance = .01f;
    private float maxZoomDistance = 10f;
    private float zoomSpeed = 50f;

    private void Awake()
    {
        // Gets transform of target (player)
        targetTransform = FindObjectOfType<PlayerController>().transform;
        // 'cameraTransform' equals the main unity camera transform
        cameraTransform = Camera.main.transform;
        // The default position of the camera is the main camera's local 'Z' position
        defaultPosition = cameraTransform.localPosition.z;

    }

    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position + (cameraTransform.forward * zoomDistance) + new Vector3(0,10,0), ref cameraFollowVelocity, cameraFollowSpeed);
        transform.position = targetPosition;
    }

    private void RotateCamera(Vector2 cameraInput)
    {
        // Control Camera Speed
        vertAxisAngle += cameraInput.x * cameraHorizSpeed;
        horizAxisAngle -= cameraInput.y * cameraVertSpeed;

        // Make sure horizAxisAngle cannot go above or below min and max, respectively
        horizAxisAngle = Mathf.Clamp(horizAxisAngle,minHorizAxisAngle,maxHorizAxisAngle);
        
        // Set Rotation variable for the camera
        Vector3 rotation = Vector3.zero;

        rotation.y = vertAxisAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = horizAxisAngle;

        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {
        /* float targetPosition = defaultPosition;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();
        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out RaycastHit hit, Mathf.Abs(targetPosition), collisionLayers))
        { 
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition = -distance - cameraCollisionOffset;
        }
        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition -= minimumCollisionOffset;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition; */
    }

    private void CameraZoom(Vector2 cameraScrollValue) {

        Debug.Log(cameraScrollValue);
        zoomDistance += cameraScrollValue.y * Time.deltaTime * zoomSpeed;
        zoomDistance = Mathf.Clamp(zoomDistance,minZoomDistance,maxZoomDistance);
        //Debug.Log(zoomDistance);
        transform.position = targetTransform.position - new Vector3(transform.right.x *zoomDistance, transform.up.y, transform.forward.z);


    }

    public void HandleAllCameraMovement(Vector2 cameraInput, Vector2 cameraScrollValue)
    {
        FollowTarget();
        RotateCamera(cameraInput);
        HandleCameraCollisions();
        CameraZoom(cameraScrollValue);
    }

}