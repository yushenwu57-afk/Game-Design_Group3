using UnityEngine;

public class Finish : MonoBehaviour
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

        if (other.TryGetComponent<Player>(out var player))
        {
            if (!player.hasKey) return;

            hasTriggered = true;
            player.FreezeOnWin();

            if (winUI != null)
            {
                winUI.SetActive(true);
            }

            Time.timeScale = 0f;

            if (disablePlayerOnWin)
            {
                player.enabled = false;
            }
        }
    }
}
