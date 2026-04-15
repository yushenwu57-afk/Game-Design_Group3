using UnityEngine;

public class Key : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        if (other.TryGetComponent<Player>(out var player))
        {
            player.GiveKey();
            Destroy(gameObject);
        }
    }
}
