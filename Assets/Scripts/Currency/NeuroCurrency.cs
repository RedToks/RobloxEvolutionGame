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

    public void AddCoinCurrency(long amount)
    {
        // Проверяем, активирован ли x2 доход для NeuroCoins
        if (BuyController.isDoubleNeuroEarningsActive)
        {
            amount *= 2; // Удваиваем доход
        }

        coinCurrency += amount;
        UpdateUI();
    }

    public void SpendCoinCurrency(long amount)
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
