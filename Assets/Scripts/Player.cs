using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{


    public float speed = 5f; // Speed of the player movement
    public float jumpForce = 6f; // Jump strength
    public int maxJumps = 2; // Total jumps allowed (2 = double jump)
    public float secondJumpMultiplier = 0.6f; // Lower height for the second jump
    public Transform groundCheck; // Ground check point
    public float groundCheckRadius = 0.1f; // Ground check radius
    public LayerMask groundLayer; // What counts as ground
    public string damageTag = "Damage"; // Tag for damage objects
    public int maxHp = 3; // Player max HP
    public int damageAmount = 1; // HP lost per hit
    public float damageJumpForce = 8f; // Jump force when taking damage
    public float hurtColorTime = 0.2f; // How long to stay red
    public float damageCooldown = 0.2f; // Prevent rapid repeated hits
    public Transform respawnPoint; // Where to respawn after falling
    public float fallY = -10f; // Y threshold to trigger respawn
    public bool hasKey = false; // Whether the player currently holds a key
    public Health healthUI; // UI hearts controller (assign from Canvas)

    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Animator animator;
    private bool isGrounded;
    private string currentAnim;
    private SpriteRenderer spriteRenderer;
    private Color originalColor = Color.white;
    private int currentHp;
    private float lastDamageTime = -999f;
    private int jumpsRemaining;
    private float moveInput;
    private bool jumpRequested;
    private MovingPlatform currentPlatform;

    private const string AnimIdle = "Player";
    private const string AnimRun = "Player_Run";
    private const string AnimJump = "Player_Jump";
    private const string AnimFall = "Player_Fall";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        maxJumps = Mathf.Clamp(maxJumps, 1, 2);
        currentHp = maxHp;
        jumpsRemaining = maxJumps;
        if (healthUI == null)
        {
            healthUI = FindFirstObjectByType<Health>();
        }
    }

    void OnValidate()
    {
        maxJumps = Mathf.Clamp(maxJumps, 1, 2);
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) moveInput = -1f;
            if (Keyboard.current.dKey.isPressed) moveInput = 1f;
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                jumpRequested = true;
            }
        }
        // Face the movement direction. Assume the original sprite faces right;
        // flip when moving to the left.
        if (spriteRenderer != null && Mathf.Abs(moveInput) > 0.01f)
        {
            spriteRenderer.flipX = moveInput < 0f;
        }

        UpdateAnimation(moveInput);

        if (transform.position.y < fallY)
        {
            FallOut();
        }
    }

    void FixedUpdate()
    {
        float platformX = currentPlatform != null ? currentPlatform.CurrentVelocity.x : 0f;
        rb.linearVelocity = new Vector2(moveInput * speed + platformX, rb.linearVelocity.y); // Apply horizontal movement to Rigidbody2D
        isGrounded = groundCheck != null && Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
        {
            jumpsRemaining = maxJumps;
        }
        if (jumpRequested && jumpsRemaining > 0)
        {
            float force = jumpForce;
            if (jumpsRemaining == 1 && maxJumps >= 2)
            {
                force *= Mathf.Clamp01(secondJumpMultiplier);
            }
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
            jumpsRemaining--;
        }
        jumpRequested = false;
    }

    private void UpdateAnimation(float moveInput)
    {
        if (animator == null) return;

        if (!isGrounded)
        {
            if (rb.linearVelocity.y > 0.01f)
            {
                PlayAnim(AnimJump);
            }
            else
            {
                PlayAnim(AnimFall);
            }
            return;
        }

        if (Mathf.Abs(moveInput) > 0.01f)
        {
            PlayAnim(AnimRun);
        }
        else
        {
            PlayAnim(AnimIdle);
        }
    }

    private void PlayAnim(string animName)
    {
        if (currentAnim == animName) return;
        animator.Play(animName);
        currentAnim = animName;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleDamage(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleDamage(collision.collider);
    }

    private void HandleDamage(Collider2D other)
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

    private void TakeDamage(int amount, bool applyKnockback)
    {
        currentHp = Mathf.Max(0, currentHp - amount);
        if (healthUI != null)
        {
            healthUI.TakeDamage(amount);
        }
        else
        {
            Debug.LogWarning($"{name}: Health UI not assigned.");
        }

        if (currentHp <= 0)
        {
            Respawn();
            currentHp = maxHp;
            if (healthUI != null)
            {
                healthUI.Heal(maxHp);
            }
            return;
        }

        if (applyKnockback)
        {
            // Jump up when hurt
            float upVelocity = Mathf.Max(rb.linearVelocity.y, damageJumpForce);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, upVelocity);
        }

        // Flash red
        if (spriteRenderer != null)
        {
            StopAllCoroutines();
            StartCoroutine(HurtFlash());
        }
    }

    private System.Collections.IEnumerator HurtFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hurtColorTime);
        spriteRenderer.color = originalColor;
    }

    private void FallOut()
    {
        int hpBefore = currentHp;
        TakeDamage(1, false);
        if (hpBefore > 1)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        Vector3 targetPos = respawnPoint != null ? respawnPoint.position : Vector3.zero;
        transform.position = targetPos;
        rb.linearVelocity = Vector2.zero;
    }

    public void GiveKey()
    {
        hasKey = true;
    }

    public void ResetForRestart()
    {
        currentHp = maxHp;
        jumpsRemaining = maxJumps;
        hasKey = false;
        lastDamageTime = -999f;
        moveInput = 0f;
        jumpRequested = false;

        if (healthUI == null)
        {
            healthUI = FindFirstObjectByType<Health>();
        }
        if (healthUI != null)
        {
            healthUI.ResetToFull();
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Respawn();
    }

    public void ReturnToStart()
    {
        Respawn();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TrySetPlatform(collision.collider);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        ClearPlatform(collision.collider);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TrySetPlatform(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ClearPlatform(other);
    }

    private void TrySetPlatform(Collider2D other)
    {
        if (other == null) return;
        MovingPlatform platform = other.GetComponentInParent<MovingPlatform>();
        if (platform != null) currentPlatform = platform;
    }

    private void ClearPlatform(Collider2D other)
    {
        if (other == null) return;
        MovingPlatform platform = other.GetComponentInParent<MovingPlatform>();
        if (platform != null && currentPlatform == platform) currentPlatform = null;
    }
}
