using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flecha : MonoBehaviour
{
    [SerializeField] private float tempoDeVida;
    [SerializeField] private CharacterMovement character;

    //private void OnEnable()
    //{
    //    character.flecha.Enqueue(this.gameObject);
    //    Debug.Log(character.flecha.Peek().name);
    //    //StartCoroutine(DesativarFlecha());
    //}

    //private void OnDisable()
    //{
    //    //character.flecha.Enqueue(this.gameObject);
    //    character.flecha.Enqueue(this.gameObject);
    //    Debug.Log(character.flecha.Peek().name);
    //}

    public void Desativar()
    {
        StartCoroutine(DesativarFlecha());
    }

    public IEnumerator DesativarFlecha()
    {
        yield return new WaitForSeconds(tempoDeVida);
        character.flecha.Enqueue(this.gameObject);
        this.gameObject.SetActive(false);
    }
}
