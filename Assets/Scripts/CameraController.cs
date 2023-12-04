using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[ExecuteAlways]
public class CameraController : MonoBehaviour
{
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float targetHeight = 1f;
    [SerializeField] private Vector2 xRotationRange = new Vector2(-50, 50);

    private Vector2 targetLook;

    public Quaternion LookRotation => cameraTarget.rotation;
    private void LateUpdate()
    {
        cameraTarget.transform.position = characterMovement.transform.position + Vector3.up * targetHeight;
      
        cameraTarget.transform.rotation = Quaternion.Euler(targetLook.x, targetLook.y, 0);
        
    }

    public void IncrementLookRotation(Vector2 lookDelta)
    {
        targetLook += lookDelta;
        targetLook.x = Mathf.Clamp(targetLook.x, xRotationRange.x, xRotationRange.y);
    }
}
