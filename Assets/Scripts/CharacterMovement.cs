using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct CharacterMovementInput
{
    public Vector2 MoveInput;
    public Quaternion LookRotation;
    public bool WantsToJump;
}

[RequireComponent(typeof(KinematicCharacterMotor))]
public class CharacterMovement : MonoBehaviour, ICharacterController
{
    public KinematicCharacterMotor motor;
    [Header("Fator Exito - Derrota")]
    private int inimigosMortos = 0;
    [SerializeField] private int vida = 10;

    [Header("Animator")]
    [SerializeField] Animator anim;

    [Header("Movimento - Terra")]
    [SerializeField] private float velocidadeCorrendo;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight = 1.5f;

    [Range(0.01f, 0.3f)]
    [SerializeField] private float JumpRequestDuration = 0.1f;

    private Vector3 moveInput;
   // private bool WantsToJump;
    private float jumpRequestExpireTime;

    [Header("Movimento - Ar")]
    [SerializeField] private float airMaxSpeed = 3f;
    [SerializeField] private float airAcceleration = 20f;
    [Min(0)]
    [SerializeField] private float drag = 0.5f;

    [SerializeField] private float jumpForce => Mathf.Sqrt(2 * gravity * jumpHeight);

    public int InimigosMortos { get => inimigosMortos; set => inimigosMortos = value; }

    private bool isJump, isWalk, isAttack, isRun;

    [Header("Atirar")]
    [SerializeField] private Transform spawn;
    [SerializeField] private float velocidadeFlecha;
    [SerializeField] private List<GameObject> flechas;
    public Queue<GameObject> flecha = new Queue<GameObject>();

    
    private float velocidadeNormal;
    private bool isDied = false;



    private void Awake()
    {
        PlayerPrefs.SetInt("isDead", 0);
        isJump = false;
        isWalk = false;
        isAttack = false;
        isRun = false;
       
        velocidadeNormal = maxSpeed;

        motor.CharacterController = this;
        foreach(GameObject obj in flechas)
        {
            flecha.Enqueue(obj);
        }
    }

    void Update()
    {
        UpdateAnimator();
        if (Input.GetMouseButtonDown(0) && !isAttack)
        {
            StartCoroutine(Bullet());
        }
    }

    public void SetInput(in CharacterMovementInput input)
    {
        moveInput = Vector3.zero;
        if(input.MoveInput != Vector2.zero && !isDied)
        {
            moveInput = new Vector3(input.MoveInput.x, y: 0, input.MoveInput.y);
            moveInput = input.LookRotation * moveInput;
            moveInput.y = 0;
            moveInput.Normalize();
        }

        if(input.WantsToJump && !isDied)
        {
            jumpRequestExpireTime = Time.time + JumpRequestDuration;
            //WantsToJump = input.WantsToJump;
        }
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (moveInput != Vector3.zero && !isDied)
        {
            isWalk = true;
            if(Input.GetKey(KeyCode.LeftShift))
            {
                maxSpeed = velocidadeCorrendo;
                isRun = true;
            }
            else
            {
                maxSpeed = velocidadeNormal;
                isRun = false;
            }
            var targetRotation = Quaternion.LookRotation(moveInput);
            currentRotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * deltaTime);
        }
        else
        {
            isRun = false;
            isWalk = false;
            maxSpeed = velocidadeNormal;
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if(motor.GroundingStatus.IsStableOnGround && !isDied)
        {
            var targetVelocity = moveInput * maxSpeed;
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * deltaTime);
            
            if(Time.time < jumpRequestExpireTime)
            {
                isJump = true;
                currentVelocity.y = jumpForce;
                jumpRequestExpireTime = 0;
                //WantsToJump = false;
                motor.ForceUnground();
            }
            else
            {
                isJump = false;
            }
        }
        else
        {
            Vector2 targetVelocityXZ = new Vector2( moveInput.x, moveInput.z) * airMaxSpeed;
            Vector2 currentVelocityXZ = new Vector2(currentVelocity.x, currentVelocity.z);
            
            currentVelocityXZ = Vector2.MoveTowards(currentVelocityXZ, targetVelocityXZ, airAcceleration * deltaTime);
            currentVelocity.x = ApplyDrag(currentVelocityXZ.x, drag, deltaTime);
            currentVelocity.z = ApplyDrag(currentVelocityXZ.y, drag, deltaTime);
            currentVelocity.y -= gravity * deltaTime;
        }
       
    }

    private static float ApplyDrag(float v, float drag, float deltaTime)
    {
        return v * (1f / (1f + drag * deltaTime));
    }

    private void UpdateAnimator()
    {
        anim.SetBool("isJump", isJump);
        anim.SetBool("isWalk", isWalk);
        anim.SetBool("isAttack", isAttack);
        anim.SetBool("isRun", isRun);
    }

    public IEnumerator Bullet()
    {

        transform.eulerAngles += new Vector3(0, 45f, 0);
        isAttack = true;
        GameObject flechaA = flecha.Dequeue();
        flechaA.SetActive(true);
        flechaA.GetComponent<Flecha>().enabled = true;
        flechaA.transform.position = spawn.position;
        flechaA.transform.rotation = spawn.rotation;
        flechaA.GetComponent<Rigidbody>().velocity = spawn.forward * velocidadeFlecha;
        flechaA.GetComponent<Flecha>().Desativar();

        yield return new WaitForSeconds(1f);
        isAttack = false;
        //transform.eulerAngles += new Vector3(0, -45, 0);

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "DanoZ" && !isDied)
        {
            Debug.Log("Levei dano");
            vida--;
            if (vida<=0 && !isDied)
            {
                isDied = true;
                anim.Play("Player_Death", 0);
                StartCoroutine(RestartCena());
                PlayerPrefs.SetInt("isDead", 1);
            }
        }
    }

    IEnumerator RestartCena()
    {
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene(0);
    }

    public void AlterarColisor()
    {
        motor.SetCapsuleDimensions(0.25f, 0.5f, 1f);
        gameObject.tag = "Untagged";
        gameObject.layer = 11;
    }

    #region SemUso
    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public void AfterCharacterUpdate(float deltaTime)
    {

    }

    public void BeforeCharacterUpdate(float deltaTime)
    {

    }
     
    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {

    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void PostGroundingUpdate(float deltaTime)
    {
 
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }
    #endregion
}
