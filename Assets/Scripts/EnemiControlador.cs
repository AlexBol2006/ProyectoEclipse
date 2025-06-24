using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiControlador : MonoBehaviour
{
    [Header("ReferenciasE")]

    private Rigidbody2D rb;

    [Header("MovimientoE")]

    [SerializeField] private float velocidadMovimientoEBase;
    [SerializeField] private float velocidadMovimientoEActual;
    [SerializeField] private Transform controladorFrente;
    [SerializeField] private float distanciaRaycast;
    [SerializeField] private LayerMask capaSuelo;
    public bool tocandoSueloFrente;

    private void Update()
    {
        tocandoSueloFrente = Physics2D.Raycast(controladorFrente.position, transform.right * -1, distanciaRaycast, capaSuelo);
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(velocidadMovimientoEActual, rb.linearVelocity.y);

        if (tocandoSueloFrente)
        {
            velocidadMovimientoEActual *= -1;

            GirarE();
        }
        MirarDireccionMovimiento();
    }
    private void MirarDireccionMovimiento()
    {
        if ((velocidadMovimientoEActual > 0 && !MirarDerecha()) || (velocidadMovimientoEActual < 0 && MirarDerecha()))
        {
            GirarE();
        }
    }
    private void GirarE()
    {
        Vector3 rotacion = transform.eulerAngles;
        rotacion.y = rotacion.y == 0 ? 180 : 0;
        transform.eulerAngles = rotacion;
    }
    private bool MirarDerecha()
    {
        return transform.eulerAngles.y == 180;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(controladorFrente.position, controladorFrente.position + distanciaRaycast * transform.right * -1);

    }
}
