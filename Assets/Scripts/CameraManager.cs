using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform targetTransform; // The object the camera will follow
    public Transform cameraPivot; // The object the camera uses to pivot (Look up and down)
    public Transform cameraTransform; //The transform of the actual camrea in the scene
    public LayerMask collisionLayers; // The layers we want our camera to collide with
    private float defaultPosition;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPosition;

    public float cameraCollisionOffset = 0.2f; // How much camera will repel from objects
    public float minimumCollisionOffset = 0.2f;
    public float cameraCollisionRadius = 2f;
    public float cameraFollowSpeed = 0.2f;
    public float cameraLookSpeed = 2f;
    public float cameraPivotSpeed = 2f;

    public float lookAngle; //Camera looking up and down
    public float pivotAngle; //Camera looking left and right
    public float minimumPivotAngle = -35;
    public float maximumPivotAngle = 35;
    private void Awake()
    {
        targetTransform = FindObjectOfType<PlayerController>().transform;
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);

        transform.position = targetPosition;
    }

    private void RotateCamera(Vector2 cameraInput)
    {
        lookAngle += cameraInput.x * cameraLookSpeed;
        pivotAngle -= cameraInput.y * cameraPivotSpeed;

        if (pivotAngle >= maximumPivotAngle) pivotAngle = maximumPivotAngle;
        if (pivotAngle <= minimumPivotAngle) pivotAngle = minimumPivotAngle;
        
        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        //rotation = Vector3.zero;
        rotation.x = pivotAngle;

        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }
    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
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
        cameraTransform.localPosition = cameraVectorPosition;
    }
    public void HandleAllCameraMovement(Vector2 cameraInput)
    {
        FollowTarget();
        RotateCamera(cameraInput);
        HandleCameraCollisions();
    }
}