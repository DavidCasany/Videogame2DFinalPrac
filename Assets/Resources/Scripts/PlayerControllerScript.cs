using UnityEngine;

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
    private float apuntatTimer;
    private bool estaApuntant = false;
    private bool habilitatUsadaAire = false;

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

    // Internes
    private Rigidbody2D rb;
    private Camera cam;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool facingRight = false;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        rb.gravityScale = 3.5f;
        wallDetachTimer = timeToDetach;

        if (flexaPare != null) flexaPare.SetActive(false);
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, wallLayer);

        if (isGrounded)
        {
            jumpsLeft = maxJumps;
            habilitatUsadaAire = false;
        }

        if (Input.GetKeyDown(KeyCode.W) && !isGrounded && !habilitatUsadaAire && !estaApuntant)
        {
            IniciarApuntat();
        }

        if (estaApuntant)
        {
            ActualitzarApuntat();
            return;
        }

        HandleWallLogic();

        if (!isWallSliding || isGrounded)
        {
            if (moveInput.x > 0 && !facingRight) Flip();
            else if (moveInput.x < 0 && facingRight) Flip();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleJump();
        }
    }

    void FixedUpdate()
    {
        if (estaApuntant)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (isGrounded) ApplyGroundMovement();
        else if (isWallSliding) ApplyWallMovement();
        else ApplyAirMovement();
    }

    // --- MÈTODES DE L'HABILITAT W ---

    void IniciarApuntat()
    {
        estaApuntant = true;
        habilitatUsadaAire = true;
        apuntatTimer = duradaApuntat;

        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if (flexaPare != null) flexaPare.SetActive(true);
    }

    void ActualitzarApuntat()
    {
        apuntatTimer -= Time.unscaledDeltaTime;

        // 1. Obtenir direcció normalitzada (sempre força constant)
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 direccio = ((Vector2)mousePos - (Vector2)transform.position).normalized;

        // 2. Calcular l'angle al món
        float angle = Mathf.Atan2(direccio.y, direccio.x) * Mathf.Rad2Deg;

        if (flexaPare != null)
        {
            // SOLUCIÓ FINAL PER AL FLIP:
            // Usem .SetPositionAndRotation o .rotation directament per sobreescriure la rotació heretada del pare.
            // Si la fletxa apunta al revés, canvia "angle" per "angle + 180f"
            flexaPare.transform.rotation = Quaternion.Euler(0, 0, angle +180f);

            // Si el personatge té localScale.x negatiu, Unity inverteix l'eix visual.
            // Per corregir-ho i que la fletxa no es vegi del revés (mirall), forcem el localScale del fill
            Vector3 fletxaScale = flexaPare.transform.localScale;
            fletxaScale.x = transform.localScale.x > 0 ? 1 : -1;
            fletxaScale.y = transform.localScale.x > 0 ? 1 : -1;
            flexaPare.transform.localScale = fletxaScale;
        }

        if (apuntatTimer <= 0)
        {
            ExecutarLlançament(direccio);
        }
    }

    void ExecutarLlançament(Vector2 dir)
    {
        estaApuntant = false;
        if (flexaPare != null) flexaPare.SetActive(false);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        rb.velocity = dir * forçaLlançament;
        currentSpeed = rb.velocity.x;
    }

    // --- MÈTODES DE MOVIMENT ---

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

    public void ResetSaltsIAbilitat()
    {
        jumpsLeft = maxJumps;
        habilitatUsadaAire = false;
        // Opcionalment pots aturar qualsevol "Dash/Apuntat" si estava en curs
        estaApuntant = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        if (flexaPare != null) flexaPare.SetActive(false);
    }
}