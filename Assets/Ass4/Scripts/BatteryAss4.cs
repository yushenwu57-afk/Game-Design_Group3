using UnityEngine;

public class BatteryAss4 : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip pickupSfx;
    [SerializeField, Range(0f, 1f)] private float pickupVolume = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerAss4>() != null)
        {
            BatteryCounterAss4.CollectOne();
            if (pickupSfx != null)
            {
                AudioSource.PlayClipAtPoint(pickupSfx, transform.position, pickupVolume);
            }
            Destroy(gameObject);
        }
    }
}