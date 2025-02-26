using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using KinematicCharacterController.Examples;
using CoppraGames;

public class PetEggShop : MonoBehaviour
{
    [System.Serializable]
    public class PetDrop
    {
        public Pet pet; // Питомец
        public float dropChance; // Шанс выпадения в процентах
    }

    public List<PetDrop> pets; // Список питомцев с шансами
    public int cost = 5; // Цена попытки
    public Button buyButton; // Кнопка покупки
    public TextMeshProUGUI costText; // Текст стоимости
    public GameObject shopUI; // UI магазина
    public PetPanelUI petPanel; // Ссылка на инвентарь

    private void Start()
    {
        costText.text = cost.ToString();
        buyButton.onClick.AddListener(TryBuyPet);
        shopUI.SetActive(false);
        UpdateButtonState(); // Проверяем кнопку при старте
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ExampleCharacterController player))
        {
            shopUI.SetActive(true);
            UpdateButtonState(); // Проверяем кнопку при входе в магазин
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ExampleCharacterController player))
        {
            shopUI.SetActive(false);
        }
    }

    private void TryBuyPet()
    {
        if (NeuroCurrency.Instance.coinCurrency >= cost)
        {
            NeuroCurrency.Instance.SpendCoinCurrency(cost);
            GivePetToPlayer();
            UpdateButtonState(); // Обновляем кнопку после покупки
        }
    }

    private void GivePetToPlayer()
    {
        Pet selectedPet = GetRandomPet();
        petPanel.AddPet(selectedPet);
        PetRevealUI.Instance.ShowPet(selectedPet.Icon, selectedPet.Rarity);

        QuestManager.instance.OnAchieveQuestGoal(QuestManager.QuestGoals.OBTAIN_10_PETS);
    }

    private Pet GetRandomPet()
    {
        float totalChance = 0;
        foreach (var petDrop in pets)
        {
            totalChance += petDrop.dropChance;
        }

        float randomValue = Random.Range(0, totalChance);
        float cumulative = 0;

        foreach (var petDrop in pets)
        {
            cumulative += petDrop.dropChance;
            if (randomValue <= cumulative)
            {
                return petDrop.pet;
            }
        }

        return pets[0].pet; // На случай ошибки
    }

    private void UpdateButtonState()
    {
        buyButton.interactable = NeuroCurrency.Instance.coinCurrency >= cost;
    }
}
