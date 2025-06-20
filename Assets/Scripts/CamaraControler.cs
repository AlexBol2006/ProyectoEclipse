using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraControler : MonoBehaviour
{

    public Transform player;
    public Vector3 desplazamiento;

    private void LateUpdate()
    {
        transform.position = player.position + desplazamiento;

    }
}
