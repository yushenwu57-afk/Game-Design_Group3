using UnityEngine;
using System.Collections;

public class BreakableIcePlatform : MonoBehaviour
{
    public Transform visual;

    public float breakDelay = 3f;
    public float respawnDelay = 5f;

    public float shakeAmount = 0.06f;
    public float shakeSpeed = 20f;

    private bool triggered = false;

    private Collider2D[] cols;
    private SpriteRenderer[] renders;

    private Vector3 visualStartLocalPos;
    private Vector3 spawnPos;

    void Start()
    {
        spawnPos = transform.position;

        cols = GetComponents<Collider2D>();

        if (visual != null)
        {
            visualStartLocalPos = visual.localPosition;
            renders = visual.GetComponentsInChildren<SpriteRenderer>(true);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (triggered) return;

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
        foreach (var c in cols)
            c.enabled = false;

        foreach (var r in renders)
            r.enabled = false;
    }

    void Respawn()
    {
        transform.position = spawnPos;

        foreach (var c in cols)
            c.enabled = true;

        foreach (var r in renders)
            r.enabled = true;

        if (visual != null)
            visual.localPosition = visualStartLocalPos;

        triggered = false;
    }
}