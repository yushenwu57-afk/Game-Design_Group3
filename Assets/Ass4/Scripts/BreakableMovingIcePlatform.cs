using UnityEngine;
using System.Collections;

public class BreakableMovingIcePlatform : MonoBehaviour
{
    [Header("Move Path")]
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 1.5f;
    public float pauseTime = 0.2f;   // 到点停顿时间

    [Header("Break")]
    public Transform visual;
    public float breakDelay = 3f;
    public float respawnDelay = 5f;

    [Header("Shake")]
    public float shakeAmount = 0.05f;
    public float shakeSpeed = 20f;

    private Collider2D[] cols;
    private SpriteRenderer[] renders;

    private Vector3 visualStartLocalPos;
    private Vector3 startPos;

    private bool movingToB = true;
    private bool paused = false;

    private bool triggered = false;
    private bool broken = false;

    void Start()
    {
        startPos = transform.position;

        if (visual == null)
            visual = transform.Find("Visual");

        cols = GetComponents<Collider2D>();

        if (visual != null)
        {
            visualStartLocalPos = visual.localPosition;
            renders = visual.GetComponentsInChildren<SpriteRenderer>(true);
        }
        else
        {
            renders = new SpriteRenderer[0];
        }
    }

    void Update()
    {
        if (!broken)
            MovePlatform();
    }

    void MovePlatform()
    {
        if (paused) return;
        if (pointA == null || pointB == null) return;

        Vector3 targetLocal =
            movingToB ? pointB.localPosition : pointA.localPosition;

        Vector3 targetWorld = startPos + targetLocal;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWorld,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetWorld) <= 0.01f)
        {
            transform.position = targetWorld;
            StartCoroutine(PauseThenSwitch());
        }
    }

    IEnumerator PauseThenSwitch()
    {
        paused = true;

        yield return new WaitForSeconds(pauseTime);

        movingToB = !movingToB;
        paused = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (triggered || broken) return;

        if (collision.collider.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(BreakRoutine());
        }
    }

    IEnumerator BreakRoutine()
    {
        float timer = 0f;

        while (timer < breakDelay)
        {
            timer += Time.deltaTime;

            if (visual != null)
            {
                float offsetY =
                    Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;

                visual.localPosition =
                    visualStartLocalPos + new Vector3(0f, offsetY, 0f);
            }

            yield return null;
        }

        BreakNow();

        yield return new WaitForSeconds(respawnDelay);

        Respawn();
    }

    void BreakNow()
    {
        broken = true;

        foreach (var c in cols)
            c.enabled = false;

        foreach (var r in renders)
            r.enabled = false;
    }

    void Respawn()
    {
        transform.position = startPos;

        foreach (var c in cols)
            c.enabled = true;

        foreach (var r in renders)
            r.enabled = true;

        if (visual != null)
            visual.localPosition = visualStartLocalPos;

        movingToB = true;
        paused = false;
        triggered = false;
        broken = false;
    }
}