using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private float speed;
    [SerializeField] private float maxForce;
    [SerializeField] private float sensivity;
    [SerializeField] private float jumpForce;
    [SerializeField] private Animator anim;
   
    private Vector2 move;
    private Vector2 look;
    private float lookRotation;
    public bool isGrounded = true;

  
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void FixedUpdate()
    {
        Move();
    }
    
    private void LateUpdate()
    {
        Look();
    }

    private void Move()
    {

        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = new Vector3(move.x, 0, move.y);
        targetVelocity *= speed;

        targetVelocity = transform.TransformDirection(targetVelocity);

        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);

        Vector3.ClampMagnitude(velocityChange, maxForce);
        
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
        
        if((Input.GetAxis("Horizontal")> 0) || (Input.GetAxis("Vertical")> 0))
        {
            anim.SetBool("isWalk", true);
        }
        else
        {
            anim.SetBool("isWalk", false);
        }
    }

    private void Look()
    {
        transform.Rotate(Vector3.up * look.x * sensivity);
        lookRotation = Mathf.Clamp(lookRotation, -90, 90);
        cameraHolder.transform.eulerAngles = new Vector3(lookRotation, cameraHolder.transform.eulerAngles.y, cameraHolder.transform.eulerAngles.z);
    }
    
    private void Jump()
    {

        if(isGrounded)
        {
            rb.velocity = new Vector2(0, jumpForce);
            anim.SetBool("isJump", true);
        }
        isGrounded = false;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        Jump();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isJump", false);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isJump", false);
        }
    }
}
