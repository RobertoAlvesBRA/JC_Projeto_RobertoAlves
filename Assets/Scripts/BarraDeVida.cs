using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarraDeVida : MonoBehaviour
{
    void LateUpdate()
    {
        Vector3 lookAtTarget = transform.position + Camera.main.transform.forward;
        transform.LookAt(lookAtTarget, Camera.main.transform.up);
    }
}
