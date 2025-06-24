using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MovimientoPlayer : MonoBehaviour
{
    private const string STRING_VELOCIDAD_HORIZONTAL = "VelocidadHorizontal";
    [Header("Referencia")]
    public Rigidbody2D rb;
    [SerializeField] private Animator animator;


    [Header("Movimiento")]
    public float velocidadCaminar ; 
    public float velocidadCorrer ; 
    private float velocidadActual;        
    private float movX;
    private bool estaCorriendo = true;

    [Header("Salto")]
    public float fuerzaSalto ;
    public LayerMask entorno;
    [SerializeField] private bool enSuelo;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector2 dimensionesCaja;

    [Header("Salto Pared")]
    public float fuerzaSaltoPared = 3f;
    public float longitudRaycast = 0.1f;
    public float fuerzaHorSalto ;

    [Header("Dash")]
    public float DashCooldown;
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
        //MovimientoHorizontal
        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            estaCorriendo = !estaCorriendo; 
        }
       
        if (estaCorriendo)
        {
            velocidadActual = velocidadCorrer; 
        }
        else
        {
            velocidadActual = velocidadCaminar; 
        }

        movX = Input.GetAxis("Horizontal");
        velocidadActual = movX * velocidadActual;


        //Salto 
        if (movX < 0 && EnParedL() || movX > 0 && EnParedR())
        {
            velocidadActual = 0;
        }

              enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionesCaja, 0f, entorno);


        rb.gravityScale = EnParedR() && rb.linearVelocityY < 0 || EnParedL() && rb.linearVelocityY < 0 ? 0.1f : 1;


        if (enSuelo && Input.GetKeyDown(KeyCode.Space) )
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);

        }
        else if (EnParedR() && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(-fuerzaHorSalto, fuerzaSaltoPared), ForceMode2D.Impulse);
            StartCoroutine(XD());
        }
        else if (EnParedL() && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(fuerzaHorSalto, fuerzaSaltoPared), ForceMode2D.Impulse);
            StartCoroutine(XD());
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        MirarDireccionMovimiento();
        ControladorAnimaciones();
    }
    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        if (movX != 0)
        {
            rb.linearVelocity = new Vector2(xD ? rb.linearVelocityX : velocidadActual, rb.linearVelocity.y);
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
    private void ControladorAnimaciones()
    {
        animator.SetFloat(STRING_VELOCIDAD_HORIZONTAL, Mathf.Abs(rb.linearVelocity.x));
    }
}

