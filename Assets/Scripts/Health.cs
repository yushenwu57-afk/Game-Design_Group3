using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Health : MonoBehaviour
{
    [Header("Hearts UI")]
    [SerializeField] private Image[] hearts;
    [SerializeField] private int maxHearts = 3;
    [SerializeField] private bool debugKeys = false;
    [Header("Audio")]
    [SerializeField] private AudioClip damageSfx;
    [SerializeField, Range(0f, 1f)] private float damageVolume = 1f;
    [Header("Failure UI")]
    [SerializeField] private GameObject failureUI;

    private int currentHearts;

    private void Awake()
    {
        currentHearts = Mathf.Clamp(maxHearts, 0, hearts != null ? hearts.Length : maxHearts);
        RefreshHeartsUI();
        
        if (failureUI != null)
        {
            failureUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (!debugKeys) return;
        var kb = Keyboard.current;
        if (kb == null) return;
        if (kb.hKey.wasPressedThisFrame) TakeDamage();
        if (kb.jKey.wasPressedThisFrame) Heal();
    }

    public void TakeDamage(int amount = 1)
    {
        if (amount <= 0) return;
        currentHearts = Mathf.Max(0, currentHearts - amount);
        RefreshHeartsUI();
        if (damageSfx != null)
        {
            AudioSource.PlayClipAtPoint(damageSfx, transform.position, damageVolume);
        }
        Debug.Log($"{name}: TakeDamage({amount}) -> {currentHearts}/{maxHearts}");
        

        if (currentHearts <= 0)
        {
            if (failureUI != null)
            {
                failureUI.SetActive(true);
                Time.timeScale = 0f; 
            }
            else
            {
                Debug.LogWarning("Health: failureUI is not assigned!");
            }
        }
    }

    public void Heal(int amount = 1)
    {
        if (amount <= 0) return;
        currentHearts = Mathf.Min(maxHearts, currentHearts + amount);
        RefreshHeartsUI();
        Debug.Log($"{name}: Heal({amount}) -> {currentHearts}/{maxHearts}");
    }

    public void ResetToFull()
    {
        currentHearts = Mathf.Clamp(maxHearts, 0, hearts != null ? hearts.Length : maxHearts);
        RefreshHeartsUI();
        Debug.Log($"{name}: ResetToFull -> {currentHearts}/{maxHearts}");
    }

    [ContextMenu("Test/Take Damage")]
    private void TestTakeDamage()
    {
        TakeDamage();
    }

    [ContextMenu("Test/Heal")]
    private void TestHeal()
    {
        Heal();
    }

    private void RefreshHeartsUI()
    {
        if (hearts == null || hearts.Length == 0)
        {
            Debug.LogWarning($"{name}: Hearts array is not assigned.");
            return;
        }
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;
            hearts[i].enabled = i < currentHearts;
        }
    }
}
