using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public Transform pointA;
    public Transform pointB;
    public float waitTime = 0.2f;
    public Transform player;
    public float faceDamageDistance = 5f;
    public float faceDamageHeightTolerance = 0.2f;
    public int faceDamageAmount = 1;
    public string playerTag = "Player";

    Transform _target;
    float _waitTimer;
    float _startY;
    SpriteRenderer _sprite;
    Player _playerController;
    bool _faceDamageLocked;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null) player = playerObj.transform;
        }
        if (player != null) _playerController = player.GetComponent<Player>();
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
    }

    void Update()
    {
        TryFaceDamagePlayer();
        if (pointA == null || pointB == null) return;

        if (_waitTimer > 0f)
        {
            _waitTimer -= Time.deltaTime;
            return;
        }

        Vector3 targetPos = _target.position;
        targetPos.y = _startY;

        // Face the movement direction. Assume the original sprite faces right;
        // flip when moving to the left.
        float dir = targetPos.x - transform.position.x;
        if (_sprite != null && Mathf.Abs(dir) > 0.001f)
        {
            _sprite.flipX = dir < 0f;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) <= 0.01f)
        {
            _target = _target == pointA ? pointB : pointA;
            if (waitTime > 0f) _waitTimer = waitTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (pointA == null || pointB == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pointA.position, pointB.position);
        Gizmos.DrawSphere(pointA.position, 0.1f);
        Gizmos.DrawSphere(pointB.position, 0.1f);
    }

    void TryFaceDamagePlayer()
    {
        if (player == null || _playerController == null) return;

        float dx = player.position.x - transform.position.x;
        float dy = player.position.y - transform.position.y;
        if (Mathf.Abs(dx) > faceDamageDistance)
        {
            _faceDamageLocked = false;
            return;
        }
        if (Mathf.Abs(dy) > faceDamageHeightTolerance)
        {
            _faceDamageLocked = false;
            return;
        }

        bool facingRight = _sprite != null ? !_sprite.flipX : dx > 0f;
        bool playerOnRight = dx > 0f;
        if (facingRight != playerOnRight)
        {
            _faceDamageLocked = false;
            return;
        }

        if (_faceDamageLocked) return;
        _faceDamageLocked = true;
        _playerController.TryDamage(faceDamageAmount, true);
    }
}
