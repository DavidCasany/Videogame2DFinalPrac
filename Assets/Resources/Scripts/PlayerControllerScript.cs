using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerControllerScript : MonoBehaviour
{
    [Header("Estat: TERRA")]
    public float groundMaxSpeed = 12f;
    public float groundAcceleration = 60f;
    public float groundDeceleration = 60f;

    [Header("Estat: AIRE")]
    public float airDeceleration = 15f;
    public float airAccelerationAdd = 10f;
    public float maxAirSpeedMultiplier = 3f;

    [Header("Estat: PARET")]
    public float wallSlideSpeed = 2f;
    public Vector2 wallJumpNormal = new Vector2(12f, 15f);
    public Vector2 wallJumpVertical = new Vector2(0f, 15f);
    public float timeToDetach = 0.5f;
    private float wallDetachTimer;

    [Header("Habilitat de Llançament (W)")]
    public GameObject flexaPare;
    public float forçaLlançament = 25f;
    public float duradaApuntat = 1f;

    [Header("Combat")]
    public bool estaFentDobleSalt = false;

    [Header("Animacions Extres")]
    public float duradaAnimacioDash = 0.4f;
    private float dashAnimTimer = 0f;
    public float duradaAnimacioLanding = 0.15f;
    private float landingAnimTimer = 0f;

    [Header("Configuració Salt")]
    public float jumpForceY = 13f;
    public float jumpForceX = 4f;
    public int maxJumps = 2;
    private int jumpsLeft;

    [Header("Sensors i Capes")]
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public float checkRadius = 0.2f;

    // Variables internes de moviment i input
    private Rigidbody2D rb;
    private Camera cam;
    private Animator ar;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool facingRight = true;
    private float currentSpeed;
    private bool wasGrounded = true;
    private bool jumpRequested = false;

    // Variables de control d'estat
    private float apuntatTimer;
    private bool estaApuntant = false;
    private bool habilitatUsadaAire = false;
    private bool controlsActius = true;
    private bool estaMort = false;
    private bool estaFerit = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        ar = GetComponent<Animator>();

        rb.gravityScale = 3.5f;
        wallDetachTimer = timeToDetach;

        if (flexaPare != null) flexaPare.SetActive(false);
    }

    void Update()
    {
        if (controlsActius && !estaMort)
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(KeyCode.W) && !isGrounded && !habilitatUsadaAire && !estaApuntant)
            {
                IniciarApuntat();
            }
        }
        else
        {
            moveInput = Vector2.zero;
        }

        Collider2D colliderTerra = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // Només és terra de veritat si hem tocat alguna cosa I AQUESTA COSA NO ÉS UN ENEMIC (ni un obstacle)
        bool tocaTerraFisica = (colliderTerra != null && !colliderTerra.CompareTag("Enemic") && !colliderTerra.CompareTag("Obstacle"));

        isGrounded = tocaTerraFisica && rb.velocity.y <= 0.1f;

        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, wallLayer);

        if (dashAnimTimer > 0) dashAnimTimer -= Time.deltaTime;
        if (landingAnimTimer > 0) landingAnimTimer -= Time.deltaTime;

        if (!wasGrounded && isGrounded)
        {
            landingAnimTimer = duradaAnimacioLanding;
        }

        // --- MILLORA 2: RESET DEL DOBLE SALT CONTROLAT ---
        if (isGrounded)
        {
            jumpsLeft = maxJumps;
            habilitatUsadaAire = false;

            // Si tocàvem terra i teníem el doble salt actiu, el desactivem i avisem
            if (estaFentDobleSalt)
            {
                estaFentDobleSalt = false;
                Debug.Log("Terra tocada (isGrounded = true) -> Doble salt DESACTIVAT.");
            }
        }

        if (estaApuntant)
        {
            if (controlsActius) ActualitzarApuntat();
        }
        else if (!estaMort && !estaFerit)
        {
            HandleWallLogic();

            if (isGrounded || isWallSliding)
            {
                dashAnimTimer = 0f;
            }

            if (!isWallSliding || isGrounded)
            {
                if (moveInput.x > 0 && !facingRight) Flip();
                else if (moveInput.x < 0 && facingRight) Flip();
            }

            if (controlsActius && Input.GetKeyDown(KeyCode.Space))
            {
                jumpRequested = true;
            }
        }

        ActualitzarAnimacions();
        wasGrounded = isGrounded;
    }

    void FixedUpdate()
    {
        if (estaApuntant || estaMort || estaFerit)
        {
            if (estaApuntant) rb.velocity = Vector2.zero;
            jumpRequested = false;
            return;
        }

        if (isGrounded) ApplyGroundMovement();
        else if (isWallSliding) ApplyWallMovement();
        else ApplyAirMovement();

        if (jumpRequested)
        {
            HandleJump();
            jumpRequested = false;
        }
    }

    private void ActualitzarAnimacions()
    {
        if (estaMort) ar.SetInteger("State", 6);
        else if (estaFerit) ar.SetInteger("State", 5);
        else if (estaApuntant || dashAnimTimer > 0) ar.SetInteger("State", 4);
        else if (isWallSliding) ar.SetInteger("State", 3);
        else if (landingAnimTimer > 0 && isGrounded) ar.SetInteger("State", 7);
        else if (!isGrounded) ar.SetInteger("State", 2);
        else if (Mathf.Abs(moveInput.x) > 0) ar.SetInteger("State", 1);
        else ar.SetInteger("State", 0);
    }

    public void Morir()
    {
        if (estaMort) return;
        estaMort = true;
        DesactivarControls();
        rb.velocity = Vector2.zero;
        rb.simulated = false;
        Invoke("GameOverSequence", 2f);
    }

    private void GameOverSequence()
    {
        SceneManager.LoadScene(3);
    }

    public void ActivarEstatFerit(bool estat)
    {
        estaFerit = estat;
        if (estat) DesactivarControls();
        else ActivarControls();
    }

    public void ActivarControls()
    {
        controlsActius = true;
    }

    public void DesactivarControls()
    {
        controlsActius = false;
        moveInput = Vector2.zero;
        jumpRequested = false;

        if (estaApuntant)
        {
            estaApuntant = false;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            if (flexaPare != null) flexaPare.SetActive(false);
        }
    }

    public void ResetSaltsIAbilitat()
    {
        jumpsLeft = maxJumps;
        habilitatUsadaAire = false;
        estaApuntant = false;
        estaFentDobleSalt = false; // Assegurem apagar el doble salt al fer reset
        dashAnimTimer = 0f;
        landingAnimTimer = 0f;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        if (flexaPare != null) flexaPare.SetActive(false);
    }

    void IniciarApuntat()
    {
        estaApuntant = true;
        habilitatUsadaAire = true;
        apuntatTimer = duradaApuntat;
        dashAnimTimer = 0f;
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        if (flexaPare != null) flexaPare.SetActive(true);
    }

    void ActualitzarApuntat()
    {
        apuntatTimer -= Time.unscaledDeltaTime;
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 direccio = ((Vector2)mousePos - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direccio.y, direccio.x) * Mathf.Rad2Deg;

        if (flexaPare != null)
        {
            flexaPare.transform.rotation = Quaternion.Euler(0, 0, angle + 180f);
            Vector3 fletxaScale = flexaPare.transform.localScale;
            fletxaScale.x = transform.localScale.x > 0 ? 1 : -1;
            fletxaScale.y = transform.localScale.x > 0 ? 1 : -1;
            flexaPare.transform.localScale = fletxaScale;
        }

        if (apuntatTimer <= 0) ExecutarLlançament(direccio);
    }

    void ExecutarLlançament(Vector2 dir)
    {
        estaApuntant = false;
        if (flexaPare != null) flexaPare.SetActive(false);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        rb.velocity = dir * forçaLlançament;
        currentSpeed = rb.velocity.x;
        dashAnimTimer = duradaAnimacioDash;
    }

    private void ApplyGroundMovement()
    {
        float targetSpeed = moveInput.x * groundMaxSpeed;
        float speedDif = targetSpeed - rb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? groundAcceleration : groundDeceleration;
        rb.AddForce(speedDif * accelRate * Vector2.right, ForceMode2D.Force);
        currentSpeed = rb.velocity.x;
    }

    private void ApplyAirMovement()
    {
        float currentX = rb.velocity.x;
        if (moveInput.x != 0)
        {
            if (Mathf.Sign(moveInput.x) != Mathf.Sign(currentX) && Mathf.Abs(currentX) > 0.1f)
                rb.AddForce(moveInput.x * airDeceleration * Vector2.right, ForceMode2D.Force);
            else if (Mathf.Abs(currentX) < groundMaxSpeed * maxAirSpeedMultiplier)
                rb.AddForce(moveInput.x * airAccelerationAdd * Vector2.right, ForceMode2D.Force);
        }
    }

    private void ApplyWallMovement()
    {
        rb.velocity = new Vector2(0, -wallSlideSpeed);
    }

    private void HandleWallLogic()
    {
        if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            bool pushingAway = (facingRight && moveInput.x < 0) || (!facingRight && moveInput.x > 0);
            if (pushingAway) wallDetachTimer -= Time.deltaTime;
            else wallDetachTimer = timeToDetach;
            isWallSliding = (wallDetachTimer > 0);
        }
        else
        {
            isWallSliding = false;
            wallDetachTimer = timeToDetach;
        }
    }

    private void HandleJump()
    {
        if (isTouchingWall && !isGrounded)
        {
            bool paretALaDreta = facingRight;
            bool inputCapAFora = (paretALaDreta && moveInput.x < 0) || (!paretALaDreta && moveInput.x > 0);

            if (inputCapAFora)
            {
                float dirX = paretALaDreta ? -1 : 1;
                rb.velocity = new Vector2(wallJumpNormal.x * dirX, wallJumpNormal.y);
                Flip();
            }
            else rb.velocity = new Vector2(0, wallJumpVertical.y);

            return;
        }

        if (isGrounded || jumpsLeft > 0)
        {
            // --- MILLORA 3: ACTIVACIÓ DEL DOBLE SALT I DEBUG ---
            if (!isGrounded)
            {
                estaFentDobleSalt = true;
                Debug.Log("DOBLE SALT ACTIVAT! Llest per trepitjar l'enemic.");
            }

            jumpsLeft--;
            float boostX = moveInput.x * jumpForceX;
            rb.velocity = new Vector2(rb.velocity.x + boostX, jumpForceY);
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    public void AplicarRebotAlMatarEnemic(float forcaRebot)
    {
        rb.velocity = new Vector2(rb.velocity.x, forcaRebot);
        estaFentDobleSalt = false;
        ResetSaltsIAbilitat();
    }
}