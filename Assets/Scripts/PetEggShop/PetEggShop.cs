using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using KinematicCharacterController.Examples;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ExampleCharacterController player))
        {
            shopUI.SetActive(true);
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
        int coins = PlayerPrefs.GetInt("Coins", 0);
        if (coins >= cost)
        {
            PlayerPrefs.SetInt("Coins", coins - cost);
            GivePetToPlayer(); // ��������� ������� ��� ��������
        }
    }

    private void GivePetToPlayer()
    {
        Pet selectedPet = GetRandomPet();
        petPanel.AddPet(selectedPet); // ��������� ������� � ���������
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
}
