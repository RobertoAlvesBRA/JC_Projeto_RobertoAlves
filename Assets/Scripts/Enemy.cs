using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float distanciaPerseguir;
    [SerializeField] private int vida;
    [SerializeField] private Animator anim;
    [SerializeField] private ParticleSystem sangue;
    [SerializeField] private NavMeshAgent agente;
    [SerializeField] private GameObject danoZ;

    [Header("UI")]
    [SerializeField] private Slider vidaSlider;

    [Header("Player")]
    [SerializeField] private Transform player;

    private float aceleracao;
    private bool perseguir = false;
    private bool dano = false;
    private bool isAttack = false;
    private bool isDead = false;

    private void Start()
    {
        aceleracao = Random.Range(0.5f, 1.2f);
        vidaSlider.value = vida;
        anim.SetBool("Perseguindo", perseguir);
    }

    private void Update()
    {
        if (!dano && !isDead && PlayerPrefs.GetInt("isDead") == 0)
        {
            transform.LookAt(player.position);
            agente.destination = player.position;

            if (Vector3.Distance(transform.position, player.position) < distanciaPerseguir)
            {
                perseguir = true;
                agente.speed = 3f * aceleracao;
                if (Vector3.Distance(transform.position, player.position) < 1.5f && !isAttack)
                {
                    isAttack = true;
                    anim.SetBool("isAttack", isAttack);
                }
            }
            else
            {
                perseguir = false;
                agente.speed = 0.5f * aceleracao;
            }
            anim.SetBool("Perseguindo", perseguir);

        }
        else
        {
            perseguir = false;
            anim.SetBool("Perseguindo", perseguir);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Flecha")
        {
            StartCoroutine(Dano());
        }
    }

    public IEnumerator Dano()
    {
        dano = true;
        sangue.Emit(5);
        if (vida > 1)
        {
            anim.SetBool("Hit", true);
            yield return new WaitForSeconds(1f);
            dano = false;
            anim.SetBool("Hit", false);
            perseguir = true;
            anim.SetBool("Perseguindo", perseguir);
        }
        else
        {
            anim.SetBool("Dead", true);
        }
        vida--;
        vidaSlider.value = vida;
    }

    public void Morte()
    {
        vidaSlider.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        anim.SetBool("Dead", false);
        player.gameObject.GetComponent<CharacterMovement>().InimigosMortos += 1;
        if(player.gameObject.GetComponent<CharacterMovement>().InimigosMortos>9)
        {
            Debug.Log("Voce Venceu");
        }
    }

    public void Reviver(Transform spawn)
    {
        vida = 3;
        this.transform.position = spawn.position;
        vidaSlider.gameObject.SetActive(true);
    }

    IEnumerator Atacar()
    {
        danoZ.SetActive(true);      
        yield return new WaitForSeconds(0.3f);
        danoZ.SetActive(false);
        isAttack = false;
        anim.SetBool("isAttack", isAttack);
    }

    public void AlterarColisor()
    {
        isDead = true;
        this.agente.enabled = false;
        this.gameObject.GetComponent<CapsuleCollider>().height = 0.5f;
        this.gameObject.GetComponent<CapsuleCollider>().radius = 0.3f;
    }
}
