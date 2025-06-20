using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MovimientoPlayer : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadx = 0.5f;
    float velocidadX;
    private float movX;

    [Header("Salto")]
    public float fuerzaSalto = 6f;
    public LayerMask entorno;
    [SerializeField]private bool enSuelo;
    private bool recibiendoDaño;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector2 dimensionesCaja;
    private bool entradaSalto;

    [Header("Salto Pared")]    
    public float fuerzaSaltoPared = 3f;
    public Rigidbody2D rb;
    public float longitudRaycast = 0.1f;

    [Header("Dash")]
    public float DashCooldown;
    public GameObject dashParticle;
    public float dashForce = 20;
    bool canDash, isDashing;
    float dashingTime = 0.2f;
    
    void Start()
    {
        canDash = true;
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
       
        movX = Input.GetAxis("Horizontal");
        velocidadX = movX * velocidadx;

        if (Input.GetButtonDown("Jump"))
        {
            entradaSalto = true;
        }

        enSuelo =  Physics2D.OverlapBox(controladorSuelo.position, dimensionesCaja, 0f,entorno);


        if (enSuelo && Input.GetKeyDown(KeyCode.Space)&& !recibiendoDaño)
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);

        }
        if (EnParedR() && Input.GetKeyDown(KeyCode.Space) && !recibiendoDaño)
        {
            rb.AddForce(new Vector2(-fuerzaSaltoPared, fuerzaSaltoPared), ForceMode2D.Impulse);
        }
        else if (EnParedL() && Input.GetKeyDown(KeyCode.Space) && !recibiendoDaño)
        {
            rb.AddForce(new Vector2(fuerzaSaltoPared, fuerzaSaltoPared), ForceMode2D.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !recibiendoDaño)
        {
            StartCoroutine(Dash());
        }
        


    }
    private void FixedUpdate()
    {
        if(isDashing)
        {
            return;
        }

        if (movX != 0)
        {
            rb.linearVelocity = new Vector2(velocidadX, rb.linearVelocity.y);
        }
        
    }
    bool EnParedR()
    {
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, longitudRaycast,entorno);
       
        return hitRight;

    }
    bool EnParedL()
    {
       
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, longitudRaycast, entorno);
        return hitLeft;

    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.right * dashForce * movX;
        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(DashCooldown);
        canDash = true;
    }
       void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(controladorSuelo.position,dimensionesCaja);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z));
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z));
    }
}

