using UnityEngine;

public class VisionConeDamage : MonoBehaviour
{
    public int damageAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerAss4 player = other.GetComponent<PlayerAss4>();

            if (player != null)
            {
                player.TryDamage(damageAmount, true);
            }
        }
    }
}