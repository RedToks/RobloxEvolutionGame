using UnityEngine;
using TMPro;

public class BrainCurrency : MonoBehaviour
{
    public static BrainCurrency Instance;
    public long brainCurrency { get; private set; }
    public TextMeshProUGUI brainCurrencyText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddBrainCurrency(int amount) => AddBrainCurrency((long)amount);
    public void SpendBrainCurrency(int amount) => SpendBrainCurrency((long)amount);

    public void AddBrainCurrency(long amount)
    {
        brainCurrency += amount;
        UpdateUI();
    }

    public void SpendBrainCurrency(long amount)
    {
        if (brainCurrency >= amount)
        {
            brainCurrency -= amount;
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough BrainCurrency!");
        }
    }

    private void UpdateUI()
    {
        if (brainCurrencyText != null)
        {
            brainCurrencyText.text = CurrencyFormatter.FormatCurrency(brainCurrency);
        }
    }
}