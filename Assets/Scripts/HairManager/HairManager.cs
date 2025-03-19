using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using YG;

public class HairManager : MonoBehaviour
{
    public List<Hair> hairs;
    public Transform contentPanel;
    public GameObject hairButtonPrefab;
    public HairColorPicker hairColorPicker;


    private void Start()
    {
        LoadHairStates();
        GenerateHairButtons();

        if (YG2.saves.selectedHairIndex < 0 || YG2.saves.selectedHairIndex >= hairs.Count)
        {
            YG2.saves.selectedHairIndex = 0;
        }

        ActivateHair(YG2.saves.selectedHairIndex);
    }

    private void GenerateHairButtons()
    {
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        int selectedHair = YG2.saves.selectedHairIndex;

        for (int i = 0; i < hairs.Count; i++)
        {
            GameObject newButton = Instantiate(hairButtonPrefab, contentPanel);
            bool isSelected = (i == selectedHair);
            newButton.GetComponent<HairButton>().Setup(hairs[i], i, this, isSelected);
        }
    }

    public void BuyHair(int index, Hair.CurrencyType currencyType)
    {
        if (index >= 0 && index < hairs.Count)
        {
            Hair hair = hairs[index];
            if (hair.isPurchased)
            {
                ActivateHair(index);
                return;
            }

            bool purchaseSuccess = false;

            if (currencyType == Hair.CurrencyType.BrainCoin)
            {
                if (BrainCurrency.Instance.brainCurrency >= hair.brainCoinPrice)
                {
                    BrainCurrency.Instance.SpendBrainCurrency(hair.brainCoinPrice);
                    purchaseSuccess = true;
                }
            }
            else if (currencyType == Hair.CurrencyType.CoinCoin)
            {
                if (NeuroCurrency.Instance.coinCurrency >= hair.coinCoinPrice)
                {
                    NeuroCurrency.Instance.SpendCoinCurrency(hair.coinCoinPrice);
                    purchaseSuccess = true;
                }
            }

            if (purchaseSuccess)
            {
                MarkHairAsPurchased(index);
                GenerateHairButtons(); // ✅ Пересоздаём кнопки, чтобы обновить цену
            }
            else
            {
                Debug.Log("❌ Недостаточно валюты!");
            }
        }
    }


    private void MarkHairAsPurchased(int index)
    {
        while (YG2.saves.purchasedHairs.Length <= index)
        {
            YG2.saves.purchasedHairs += "0";
        }

        char[] hairArray = YG2.saves.purchasedHairs.ToCharArray();
        hairArray[index] = '1';
        YG2.saves.purchasedHairs = new string(hairArray);

        hairs[index].isPurchased = true; // ✅ Теперь `Setup()` сразу увидит покупку

        ActivateHair(index);
    }

    public void ActivateHair(int index)
    {
        if (index < 0 || index >= hairs.Count || YG2.saves.purchasedHairs.Length <= index || YG2.saves.purchasedHairs[index] != '1')
            return;

        foreach (Hair hair in hairs)
        {
            hair.hairPrefab.SetActive(false);
        }

        hairs[index].hairPrefab.SetActive(true);
        hairColorPicker.SetCurrentHairRenderer(hairs[index].hairPrefab);

        YG2.saves.selectedHairIndex = index; // ✅ Сохраняем выбор

        GenerateHairButtons();
    }

    private void LoadHairStates()
    {
        if (string.IsNullOrEmpty(YG2.saves.purchasedHairs))
            YG2.saves.purchasedHairs = "1"; // Первая прическа куплена по умолчанию

        for (int i = 0; i < hairs.Count; i++)
        {
            hairs[i].isPurchased = (i < YG2.saves.purchasedHairs.Length) && (YG2.saves.purchasedHairs[i] == '1');
        }

        // ✅ Проверяем, что индекс выбранных волос загружен правильно
        if (YG2.saves.selectedHairIndex < 0 || YG2.saves.selectedHairIndex >= hairs.Count)
        {
            YG2.saves.selectedHairIndex = 0;
        }
    }
}
