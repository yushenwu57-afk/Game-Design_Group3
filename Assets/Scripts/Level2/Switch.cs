using UnityEngine;

public class Switch : MonoBehaviour
{
    public Sprite leftSprite;
    public Sprite rightSprite;
    public MovingPlatform movingPlatform;

    private SpriteRenderer sr;
    private bool isOn = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (leftSprite != null)
            sr.sprite = leftSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TurnOn();
        }
    }

    void TurnOn()
    {
        if (isOn) return;

        isOn = true;

        if (rightSprite != null)
            sr.sprite = rightSprite;

        if (movingPlatform != null)
            movingPlatform.ActivatePlatform();
    }
}