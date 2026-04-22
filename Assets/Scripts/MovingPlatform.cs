using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 2f;
    public Transform pointA;
    public Transform pointB;
    public float waitTime = 0.2f;
    public string playerTag = "Player";
    public bool startActivated = true;

    Transform _target;
    float _waitTimer;
    float _startY;
    Vector3 _lastPos;
    Rigidbody2D _rb;
    bool _isActive;
    public Vector2 CurrentVelocity { get; private set; }

    protected void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb != null)
        {
            _rb.isKinematic = true;
            _rb.simulated = true;
        }
        _startY = transform.position.y;
        if (pointA != null)
        {
            Vector3 pos = transform.position;
            pos.x = pointA.position.x;
            transform.position = pos;
            _target = pointB != null ? pointB : pointA;
        }
        else if (pointB != null)
        {
            _target = pointB;
        }
        _isActive = startActivated;
        _lastPos = transform.position;
    }

    protected void FixedUpdate()
    {
        if (pointA == null || pointB == null) return;
        if (!_isActive)
        {
            CurrentVelocity = Vector2.zero;
            _lastPos = transform.position;
            return;
        }

        if (_waitTimer > 0f)
        {
            _waitTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector3 targetPos = _target.position;
        targetPos.y = _startY;

        Vector3 nextPos = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.fixedDeltaTime
        );

        if (_rb != null)
        {
            _rb.MovePosition(nextPos);
        }
        else
        {
            transform.position = nextPos;
        }

        if (Time.fixedDeltaTime > 0f)
        {
            CurrentVelocity = (transform.position - _lastPos) / Time.fixedDeltaTime;
        }
        _lastPos = transform.position;

        if (Vector3.Distance(transform.position, targetPos) <= 0.01f)
        {
            _target = _target == pointA ? pointB : pointA;
            if (waitTime > 0f) _waitTimer = waitTime;
        }
    }

    public void ActivatePlatform()
    {
        _isActive = true;
    }

    protected void OnDrawGizmosSelected()
    {
        if (pointA == null || pointB == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pointA.position, pointB.position);
        Gizmos.DrawSphere(pointA.position, 0.1f);
        Gizmos.DrawSphere(pointB.position, 0.1f);
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider != null && collision.collider.CompareTag(playerTag))
        {
            if (collision.collider.attachedRigidbody == null)
            {
                collision.collider.transform.SetParent(transform);
            }
        }
    }

    protected void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider != null && collision.collider.CompareTag(playerTag))
        {
            if (collision.collider.attachedRigidbody == null &&
                collision.collider.transform.parent != transform)
            {
                collision.collider.transform.SetParent(transform);
            }
        }
    }

    protected void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider != null && collision.collider.CompareTag(playerTag))
        {
            if (collision.collider.attachedRigidbody == null &&
                collision.collider.transform.parent == transform)
            {
                collision.collider.transform.SetParent(null);
            }
        }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.CompareTag(playerTag))
        {
            if (other.attachedRigidbody == null)
            {
                other.transform.SetParent(transform);
            }
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other != null && other.CompareTag(playerTag))
        {
            if (other.attachedRigidbody == null &&
                other.transform.parent == transform)
            {
                other.transform.SetParent(null);
            }
        }
    }
}
