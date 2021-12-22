using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private PlayerController playerController;

    public Transform targetTransform; // The object the camera will follow
    public Transform cameraPivot; // The object the camera uses to pivot (Look up and down)
    private Vector3 cameraFollowVelocity = Vector3.zero;

    public float cameraFollowSpeed = 0.2f;
    public float cameraLookSpeed = 2f;
    public float cameraPivotSpeed = 2f;

    public float lookAngle; //Camera looking up and down
    public float pivotAngle; //Camera looking left and right
    public float minimumPivotAngle = -35;
    public float maximumPivotAngle = 35;
    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        targetTransform = FindObjectOfType<PlayerController>().transform;
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);

        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        lookAngle += (playerController.cameraInput.x * cameraLookSpeed);
        pivotAngle -= (playerController.cameraInput.y * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);

        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;
        
        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
    }

}
