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
        public Pet pet; // �������
        public float dropChance; // ���� ��������� � ���������
    }

    public List<PetDrop> pets; // ������ �������� � �������
    public int cost = 5; // ���� �������
    public Button buyButton; // ������ �������
    public TextMeshProUGUI costText; // ����� ���������
    public GameObject shopUI; // UI ��������
    public PetPanelUI petPanel; // ������ �� ���������

    private void Start()
    {
        costText.text = cost.ToString();
        buyButton.onClick.AddListener(TryBuyPet);
        shopUI.SetActive(false);
        UpdateButtonState(); // ��������� ������ ��� ������
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ExampleCharacterController player))
        {
            shopUI.SetActive(true);
            UpdateButtonState(); // ��������� ������ ��� ����� � �������
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
            UpdateButtonState(); // ��������� ������ ����� �������
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

        return pets[0].pet; // �� ������ ������
    }

    private void UpdateButtonState()
    {
        buyButton.interactable = NeuroCurrency.Instance.coinCurrency >= cost;
    }
}
