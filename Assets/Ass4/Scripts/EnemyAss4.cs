using UnityEngine;

public class EnemyAss4 : MonoBehaviour
{
    [Header("Move")]
    public float speed = 2f;
    public Transform pointA;
    public Transform pointB;
    public float waitTime = 0.2f;

    [Header("Vision")]
    public Transform visionCone;   

    [Header("Damage")]
    public int damageAmount = 1;
    public float damageInterval = 1f; 

    Transform _target;
    float _waitTimer;
    float _startY;

    SpriteRenderer _sprite;

    bool playerInside = false;
    PlayerAss4 currentPlayer;
    float damageTimer = 0f;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();

        _startY = transform.position.y;

        if (pointA != null)
        {
            transform.position = pointA.position;
            _target = pointB;
        }
    }

    void Update()
    {
        Patrol();
        UpdateVisionCone();
        HandleDamage();
    }

    void Patrol()
    {
        if (pointA == null || pointB == null) return;

        if (_waitTimer > 0f)
        {
            _waitTimer -= Time.deltaTime;
            return;
        }

        Vector3 targetPos = _target.position;
        targetPos.y = _startY;

        float dir = targetPos.x - transform.position.x;

        if (_sprite != null)
            _sprite.flipX = dir < 0f;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            _target = (_target == pointA) ? pointB : pointA;
            _waitTimer = waitTime;
        }
    }

    void UpdateVisionCone()
    {
        if (visionCone == null || _sprite == null) return;

        if (_sprite.flipX)
        {
            visionCone.localPosition = new Vector3(-3f, 0f, 0f);
            visionCone.localScale = new Vector3(-6f, 3f, 1f);
        }
        else
        {
            visionCone.localPosition = new Vector3(3f, 0f, 0f);
            visionCone.localScale = new Vector3(6f, 3f, 1f);
        }
    }

    void HandleDamage()
    {
        if (!playerInside || currentPlayer == null) return;

        damageTimer -= Time.deltaTime;

        if (damageTimer <= 0f)
        {
            currentPlayer.TryDamage(damageAmount, true);
            damageTimer = damageInterval;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayer = other.GetComponent<PlayerAss4>();
            playerInside = true;
            damageTimer = 0f; 
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            currentPlayer = null;
        }
    }
}