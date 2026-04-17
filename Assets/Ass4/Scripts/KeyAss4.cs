using UnityEngine;

public class KeyAss4 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        if (other.TryGetComponent<PlayerAss4>(out var player))
        {
            player.GiveKey();
            Destroy(gameObject);
        }
    }
}
