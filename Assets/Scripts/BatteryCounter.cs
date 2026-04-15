using TMPro;
using UnityEngine;

public class BatteryCounter : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI counterText;
    [SerializeField] private string prefix = "";

    private static BatteryCounter instance;
    private int remaining;
    private int total;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        total = FindObjectsByType<Coin>().Length;
        remaining = total;
        RefreshUI();
    }

    public static void CollectOne()
    {
        if (instance == null)
        {
            Debug.LogError("BatteryCounter instance is null! Make sure BatteryCounter is in the scene.");
            return;
        }
        instance.remaining = Mathf.Max(0, instance.remaining - 1);
        Debug.Log($"Battery collected! Remaining: {instance.remaining}/{instance.total}");
        instance.RefreshUI();
    }

    private void RefreshUI()
    {
        if (counterText == null)
        {
            Debug.LogWarning($"{name}: counterText is not assigned.");
            return;
        }

        int collected = total - remaining;
        counterText.text = $"Batteries: {collected}/{total}";
    }
}
