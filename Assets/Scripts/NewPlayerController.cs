using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerController : MonoBehaviour
{
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private CameraController cameraController;
    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool wantsToJump = Input.GetKeyDown(KeyCode.Space); 

        //Debug.Log(message: $"{h},{v}");

        characterMovement.SetInput(new CharacterMovementInput() 
        {
            MoveInput = new Vector2(h, v),
            LookRotation = cameraController.LookRotation,
            WantsToJump = wantsToJump
        });

        float lookX = -Input.GetAxisRaw("Mouse Y");
        float lookY = Input.GetAxisRaw("Mouse X");

        cameraController.IncrementLookRotation(new Vector2(lookX, lookY));
    }
}
