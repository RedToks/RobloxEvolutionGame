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

    private int coins; // Текущие монеты
    private int selectedHairIndex = 0;

    private void Start()
    {
        LoadHairStates();
        GenerateHairButtons();
        ActivateHair(PlayerPrefs.GetInt("SelectedHair", 0));
    }

    private void GenerateHairButtons()
    {
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

    public void BuyHair(int index)
    {
        if (index >= 0 && index < hairs.Count)
        {
            if (hairs[index].isPurchased)
            {
                ActivateHair(index);
            }
            else if (coins >= hairs[index].price)
            {
                coins -= hairs[index].price;
                hairs[index].isPurchased = true;
                PlayerPrefs.SetInt($"Hair_{index}", 1); // Сохраняем покупку
                PlayerPrefs.SetInt("Coins", coins);
                ActivateHair(index);
            }
            GenerateHairButtons(); // Обновляем кнопки после покупки
        }
    }

    public void ActivateHair(int index)
    {
        if (hairs[index].isPurchased)
        {
            // Выключаем все волосы
            foreach (Hair hair in hairs)
            {
                hair.hairPrefab.SetActive(false);
            }

            // Включаем выбранные волосы
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
            hairs[i].isPurchased = (i == 0) || PlayerPrefs.GetInt($"Hair_{i}", 0) == 1;
        }

        if (!PlayerPrefs.HasKey("SelectedHair"))
        {
            PlayerPrefs.SetInt("SelectedHair", 0);
        }
    }
}
