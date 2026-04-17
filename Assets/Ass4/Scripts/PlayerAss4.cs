using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAss4 : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    public float jumpForce = 6f;
    public int maxJumps = 2;
    public float secondJumpMultiplier = 0.6f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public string iceTag = "Ice";

    [Header("Ice Movement")]
    public float groundAcceleration = 30f;
    public float iceAcceleration = 7f;
    public float groundDeceleration = 40f;
    public float iceDeceleration = 1.5f;

    [Header("Damage")]
    public string damageTag = "Damage";
    public int maxHp = 3;
    public int damageAmount = 1;
    public float damageJumpForce = 8f;
    public float hurtColorTime = 0.2f;
    public float damageCooldown = 0.2f;

    [Header("Respawn")]
    public Transform respawnPoint;
    public float fallY = -10f;

    [Header("Game State")]
    public bool hasKey = false;
    public Health healthUI;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    private int currentHp;
    private int jumpsRemaining;
    private float moveInput;
    private bool jumpRequested;
    private float lastDamageTime = -999f;

    private string currentAnim;

    private Color originalColor = Color.white;

    private const string AnimIdle = "Player";
    private const string AnimRun = "Player_Run";
    private const string AnimJump = "Player_Jump";
    private const string AnimFall = "Player_Fall";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        currentHp = maxHp;
        jumpsRemaining = maxJumps;

        if (healthUI == null)
            healthUI = FindAnyObjectByType<Health>();
    }

    void Update()
    {
        moveInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
                moveInput = -1f;

            if (Keyboard.current.dKey.isPressed)
                moveInput = 1f;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                jumpRequested = true;
        }

        if (spriteRenderer != null && Mathf.Abs(moveInput) > 0.01f)
            spriteRenderer.flipX = moveInput < 0f;

        UpdateAnimation();

        if (transform.position.y < fallY)
            FallOut();
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (isGrounded)
            jumpsRemaining = maxJumps;

        bool onIce = false;

        Collider2D hit = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (hit != null && hit.CompareTag(iceTag))
        {
            onIce = true;
        }

        float targetSpeed = moveInput * speed;

        float accel = onIce ? iceAcceleration : groundAcceleration;
        float decel = onIce ? iceDeceleration : groundDeceleration;

        float currentX = rb.linearVelocity.x;

        if (Mathf.Abs(moveInput) > 0.01f)
        {
            currentX = Mathf.MoveTowards(
                currentX,
                targetSpeed,
                accel * Time.fixedDeltaTime
            );
        }
        else
        {
            currentX = Mathf.MoveTowards(
                currentX,
                0f,
                decel * Time.fixedDeltaTime
            );
        }

        rb.linearVelocity = new Vector2(currentX, rb.linearVelocity.y);

        if (jumpRequested && jumpsRemaining > 0)
        {
            float force = jumpForce;

            if (jumpsRemaining == 1)
                force *= secondJumpMultiplier;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);

            jumpsRemaining--;
        }

        jumpRequested = false;
    }

    void UpdateAnimation()
    {
        if (animator == null) return;

        if (!isGrounded)
        {
            if (rb.linearVelocity.y > 0.01f)
                PlayAnim(AnimJump);
            else
                PlayAnim(AnimFall);

            return;
        }

        if (Mathf.Abs(rb.linearVelocity.x) > 0.2f)
            PlayAnim(AnimRun);
        else
            PlayAnim(AnimIdle);
    }

    void PlayAnim(string animName)
    {
        if (currentAnim == animName) return;

        animator.Play(animName);
        currentAnim = animName;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleDamage(other);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleDamage(collision.collider);
    }

    void HandleDamage(Collider2D other)
    {
        if (other == null) return;

        if (!other.CompareTag(damageTag)) return;

        TryDamage(damageAmount);
    }

    public void TryDamage(int amount, bool applyKnockback = true)
    {
        if (Time.time - lastDamageTime < damageCooldown) return;

        lastDamageTime = Time.time;
        TakeDamage(amount, applyKnockback);
    }

    void TakeDamage(int amount, bool applyKnockback)
    {
        currentHp = Mathf.Max(0, currentHp - amount);

        if (healthUI != null)
            healthUI.TakeDamage(amount);

        if (currentHp <= 0)
        {
            Respawn();
            currentHp = maxHp;

            if (healthUI != null)
                healthUI.ResetToFull();

            return;
        }

        if (applyKnockback)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                damageJumpForce
            );
        }

        if (spriteRenderer != null)
        {
            StopAllCoroutines();
            StartCoroutine(HurtFlash());
        }
    }

    System.Collections.IEnumerator HurtFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hurtColorTime);
        spriteRenderer.color = originalColor;
    }

    void FallOut()
    {
        TryDamage(1, false);
        Respawn();
    }

    void Respawn()
    {
        Vector3 target = respawnPoint != null ? respawnPoint.position : Vector3.zero;

        transform.position = target;
        rb.linearVelocity = Vector2.zero;
    }

    public void GiveKey()
    {
        hasKey = true;
    }

    public void ReturnToStart()
    {
        Respawn();
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
    public void ResetForRestart()
    {
        hasKey = false;

        transform.position =
            respawnPoint != null
            ? respawnPoint.position
            : Vector3.zero;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (healthUI != null)
            healthUI.ResetToFull();
    }
}
