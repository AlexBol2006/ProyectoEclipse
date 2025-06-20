using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MovimientoPlayer : MonoBehaviour
{
    [Header("Referencia")]
    public Rigidbody2D rb;

    [Header("Movimiento")]
    public float velocidadx = 0.5f;
    float velocidadX;
    private float movX;

    [Header("Salto")]
    public float fuerzaSalto = 6f;
    public float fuerzaHorSalto = 6f;
    public LayerMask entorno;
    [SerializeField] private bool enSuelo;
    private bool recibiendoDaño;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector2 dimensionesCaja;
    private bool entradaSalto;

    [Header("Salto Pared")]
    public float fuerzaSaltoPared = 3f;
    public float longitudRaycast = 0.1f;

    [Header("Dash")]
    public float DashCooldown;
    public GameObject dashParticle;
    public float dashForce = 20;
    bool canDash, isDashing;
    float dashingTime = 0.2f;

    bool xD = false;

    void Start()
    {
        canDash = true;
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {

        movX = Input.GetAxis("Horizontal");
        velocidadX = movX * velocidadx;

        if (movX < 0 && EnParedL() || movX > 0 && EnParedR())
        {
            velocidadX = 0;
        }

        if (Input.GetButtonDown("Jump"))
        {
            entradaSalto = true;
        }

        enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionesCaja, 0f, entorno);

        rb.gravityScale = EnParedR() && rb.linearVelocityY < 0 || EnParedL() && rb.linearVelocityY < 0 ? 0.1f : 1;


        if (enSuelo && Input.GetKeyDown(KeyCode.Space) && !recibiendoDaño)
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);

        }
        else if (EnParedR() && Input.GetKeyDown(KeyCode.Space) && !recibiendoDaño)
        {
            rb.AddForce(new Vector2(-fuerzaHorSalto, fuerzaSaltoPared), ForceMode2D.Impulse);
            StartCoroutine(XD());
        }
        else if (EnParedL() && Input.GetKeyDown(KeyCode.Space) && !recibiendoDaño)
        {
            rb.AddForce(new Vector2(fuerzaHorSalto, fuerzaSaltoPared), ForceMode2D.Impulse);
            StartCoroutine(XD());
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !recibiendoDaño)
        {
            StartCoroutine(Dash());
        }

        MirarDireccionMovimiento();
    }
    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        if (movX != 0)
        {
            rb.linearVelocity = new Vector2(xD ? rb.linearVelocityX : velocidadX, rb.linearVelocity.y);
        }
    }
    bool EnParedR()
    {
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, longitudRaycast, entorno);
        return hitRight;
    }
    bool EnParedL()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, longitudRaycast, entorno);
        return hitLeft;
    }
    IEnumerator XD()
    {
        xD = true;
        yield return new WaitForSeconds(0.1f);
        xD = false;
    }
    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(transform.eulerAngles.y == 0 ? 1 : -1, 0) * dashForce;
        yield return new WaitForSeconds(dashingTime);
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(DashCooldown);
        canDash = true;
    }

    private void MirarDireccionMovimiento()
    {
        if ((movX > 0 && !MirarDerecha()) || (movX < 0 && MirarDerecha()))
        {
            GirarE();
        }
    }
    private bool MirarDerecha()
    {
        return transform.eulerAngles.y == 0;
    }
    private void GirarE()
    {
        Vector3 rotacion = transform.eulerAngles;
        rotacion.y = rotacion.y == 0 ? 180 : 0;
        transform.eulerAngles = rotacion;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(controladorSuelo.position, dimensionesCaja);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + longitudRaycast, transform.position.y, transform.position.z));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x - longitudRaycast, transform.position.y, transform.position.z));
    }
}

