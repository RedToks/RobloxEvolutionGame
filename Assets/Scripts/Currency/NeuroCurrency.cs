using UnityEngine;
using TMPro;
using YG;
using System.Collections;

public class NeuroCurrency : MonoBehaviour
{
    public static NeuroCurrency Instance;
    public long coinCurrency { get; private set; }
    public TextMeshProUGUI coinCurrencyText;
    public event System.Action OnCurrencyChanged;

    private bool needsSave = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        StartCoroutine(AutoSaveCoroutine());
    }

    private void OnEnable()
    {
        YG2.onGetSDKData += OnDataLoaded;
    }

    private void OnDisable()
    {
        YG2.onGetSDKData -= OnDataLoaded;
    }

    private void OnDataLoaded()
    {
        Debug.Log($"[OnDataLoaded] Данные загружены. coinCurrency в saves: {YG2.saves.coinCurrency}");
        LoadCurrency();
    }

    private void LoadCurrency()
    {
        coinCurrency = YG2.saves.coinCurrency;
        Debug.Log($"[LoadCurrency] Валюта загружена: {coinCurrency}");
        UpdateUI();
    }

    private IEnumerator AutoSaveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            SaveCurrency();
        }
    }

    public void AddCoinCurrency(long amount)
    {
        coinCurrency += amount;
        needsSave = true;
        UpdateUI();
        OnCurrencyChanged?.Invoke();
    }

    public void SpendCoinCurrency(long amount)
    {
        if (coinCurrency >= amount)
        {
            coinCurrency -= amount;
            needsSave = true;
            UpdateUI();
            OnCurrencyChanged?.Invoke();
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

    private void SaveCurrency()
    {
        Debug.Log($"[SaveCurrency] Перед сохранением: coinCurrency = {coinCurrency}");
        YG2.saves.coinCurrency = coinCurrency;
    }
}
