using UnityEngine;

public class EnemyVisionFlip : MonoBehaviour
{
    public Transform visionCone;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (visionCone == null || sr == null) return;

        if (sr.flipX)
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
}
