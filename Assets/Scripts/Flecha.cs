using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flecha : MonoBehaviour
{
    [SerializeField] private float tempoDeVida;
    [SerializeField] private CharacterMovement character;

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
