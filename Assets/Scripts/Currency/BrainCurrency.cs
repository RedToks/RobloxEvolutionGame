using System;
using System.Collections;
using TMPro;
using UnityEngine;
using YG;

public class BrainCurrency : MonoBehaviour
{
    public static BrainCurrency Instance;
    public long brainCurrency { get; private set; }
    public TextMeshProUGUI brainCurrencyText;
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
        brainCurrency = YG2.saves.brainCurrency;
        StartCoroutine(AutoSaveCoroutine());
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

    public void AddBrainCurrency(long amount)
    {
        if (BuyController.isDoubleEarningsActive)
        {
            amount *= 2;
        }

        brainCurrency += amount;
        needsSave = true;
        UpdateUI();
        OnCurrencyChanged?.Invoke();
    }

    public void SpendBrainCurrency(long amount)
    {
        if (brainCurrency >= amount)
        {
            brainCurrency -= amount;
            needsSave = true;
            UpdateUI();
            OnCurrencyChanged?.Invoke();
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

    private void SaveCurrency()
    {
        Debug.Log($"[SaveCurrency] Перед сохранением: brainCurrency = {brainCurrency}");
        YG2.saves.brainCurrency = brainCurrency;
        YG2.SaveProgress();
    }
}
