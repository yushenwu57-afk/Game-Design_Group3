using UnityEngine;

public class FinishAss4 : MonoBehaviour
{
    [SerializeField] private GameObject winUI;
    [SerializeField] private bool disablePlayerOnWin = true;
    private bool hasTriggered;

    private void Awake()
    {
        if (winUI != null)
        {
            winUI.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;
        if (hasTriggered) return;

        if (other.TryGetComponent<PlayerAss4>(out var player))
        {
            if (!player.hasKey) return;

            hasTriggered = true;
            if (winUI != null)
            {
                winUI.SetActive(true);
            }

            if (disablePlayerOnWin)
            {
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                    rb.constraints = RigidbodyConstraints2D.FreezeAll;
                }

                player.enabled = false;
            }
        }
    }
}
