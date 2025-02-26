using UnityEngine;
using TMPro;

public class NeuroCurrency : MonoBehaviour
{
    public static NeuroCurrency Instance;
    public long coinCurrency { get; private set; }
    public TextMeshProUGUI coinCurrencyText;

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

    public void AddCoinCurrency(int amount) => AddCoinCurrency((long)amount);
    public void SpendCoinCurrency(int amount) => SpendCoinCurrency((long)amount);

    private void AddCoinCurrency(long amount)
    {
        coinCurrency += amount;
        UpdateUI();
    }

    private void SpendCoinCurrency(long amount)
    {
        if (coinCurrency >= amount)
        {
            coinCurrency -= amount;
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough CoinCurrency!");
        }
    }

    private void UpdateUI()
    {
        if (coinCurrencyText != null)
        {
            coinCurrencyText.text = CurrencyFormatter.FormatCurrency(coinCurrency);
        }
    }
}