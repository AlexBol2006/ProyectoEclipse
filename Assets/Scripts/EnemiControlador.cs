using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiControlador : MonoBehaviour
{
    public Transform player;
    public float deteccionEnRadio = 5.0f;
    public float velocidad = 2.0f;

    private Rigidbody2D rb;
    private Vector2 movimientos;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float ditanciaDelJugador = Vector2.Distance(transform.position, player.position);
        if (ditanciaDelJugador < deteccionEnRadio)
        {
            Vector2 direccion = (player.position - transform.position).normalized;
            movimientos = new Vector2(direccion.x, 0);
        }
        else
        {
            movimientos = Vector2.zero;
        }
        rb.MovePosition(rb.position + movimientos * velocidad * Time.deltaTime);
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        Vector2 direccionDaño = new Vector2(transform.position.x, 0);
    //        collision.gameObject.GetComponent<MovimientoPlayer>().ResibeDaño(direccionDaño, 1);
    //    }
    //}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, deteccionEnRadio);
    }
}
