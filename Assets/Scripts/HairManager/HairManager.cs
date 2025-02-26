using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class HairManager : MonoBehaviour
{
    public List<Hair> hairs; // Список причесок
    public Transform contentPanel; // Панель с кнопками
    public GameObject hairButtonPrefab; // Префаб кнопки
    public HairColorPicker hairColorPicker;

    private int selectedHairIndex = 0;

    private void Start()
    {
        LoadHairStates();
        GenerateHairButtons();
        ActivateHair(PlayerPrefs.GetInt("SelectedHair", 0));
    }

    private void GenerateHairButtons()
    {
        // Очищаем панель
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        int selectedHair = PlayerPrefs.GetInt("SelectedHair", 0);

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

            if (currencyType == Hair.CurrencyType.BrainCoin)
            {
                if (BrainCurrency.Instance.brainCurrency >= hair.brainCoinPrice)
                {
                    BrainCurrency.Instance.SpendBrainCurrency(hair.brainCoinPrice);
                    hair.isPurchased = true;
                    PlayerPrefs.SetInt($"Hair_{index}", 1);
                    ActivateHair(index);
                }
                else
                {
                    Debug.Log("Not enough BrainCoins!");
                }
            }
            else if (currencyType == Hair.CurrencyType.CoinCoin)
            {
                if (NeuroCurrency.Instance.coinCurrency >= hair.coinCoinPrice)
                {
                    NeuroCurrency.Instance.SpendCoinCurrency(hair.coinCoinPrice);
                    hair.isPurchased = true;
                    PlayerPrefs.SetInt($"Hair_{index}", 1);
                    ActivateHair(index);
                }
                else
                {
                    Debug.Log("Not enough CoinCoins!");
                }
            }
            GenerateHairButtons(); // Обновляем интерфейс после покупки
        }
    }

    public void ActivateHair(int index)
    {
        if (hairs[index].isPurchased)
        {
            // Выключаем все прически
            foreach (Hair hair in hairs)
            {
                hair.hairPrefab.SetActive(false);
            }
            // Включаем выбранную прическу
            hairs[index].hairPrefab.SetActive(true);
            hairColorPicker.SetCurrentHairRenderer(hairs[index].hairPrefab);
            selectedHairIndex = index;
            PlayerPrefs.SetInt("SelectedHair", index);
            GenerateHairButtons();
        }
    }

    private void LoadHairStates()
    {
        for (int i = 0; i < hairs.Count; i++)
        {
            // Первая прическа дается бесплатно
            hairs[i].isPurchased = (i == 0) || PlayerPrefs.GetInt($"Hair_{i}", 0) == 1;
        }

        if (!PlayerPrefs.HasKey("SelectedHair"))
        {
            PlayerPrefs.SetInt("SelectedHair", 0);
        }
    }
}
