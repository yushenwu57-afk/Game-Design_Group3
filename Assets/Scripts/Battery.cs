using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip pickupSfx;
    [SerializeField, Range(0f, 1f)] private float pickupVolume = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
        {
            BatteryCounter.CollectOne();
            if (pickupSfx != null)
            {
                AudioSource.PlayClipAtPoint(pickupSfx, transform.position, pickupVolume);
            }
            Destroy(gameObject);
        }
    }
}
