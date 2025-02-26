using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PetPanelUI : MonoBehaviour
{
    public Transform selectedPetsContainer; // ������� ����� (��������� �������)
    public Transform allPetsContainer; // ������ ����� (��� �������)
    public GameObject petUIPrefab; // ������ �������� UI ��� �������
    public TextMeshProUGUI selectedCounter; // ����� ��� �������� 4/3
    public Transform playerTransform; // ������ �� ������
    public Button equipBestButton; // ������ "��������� ������"
    public GameObject petPanel;

    public NotificationIcon notificationIcon;

    [SerializeField] private List<Pet> allPets = new List<Pet>(); // ��� �������
    private List<Pet> selectedPets = new List<Pet>(); // ��������� �������
    private Dictionary<Pet, GameObject> activePets = new Dictionary<Pet, GameObject>(); // �������� ������� � �����
    private const int maxSelectedPets = 3; // �������� ��������� ��������
    private PetOrbitManager petOrbitManager;

    void Start()
    {
        petOrbitManager = FindObjectOfType<PetOrbitManager>();

        // ����������� ������ "��������� ������" � ������
        equipBestButton.onClick.AddListener(EquipBestPets);

        UpdateUI();
    }

    private void UpdateUI()
    {
        selectedCounter.text = $"{selectedPets.Count}/{maxSelectedPets}";

        // ������� ���������� ����� �����������
        foreach (Transform child in selectedPetsContainer) Destroy(child.gameObject);
        foreach (Transform child in allPetsContainer) Destroy(child.gameObject);

        // ��������� ���� �������� �� ��������� (�� �������� � ��������)
        allPets.Sort((a, b) => b.Power.CompareTo(a.Power));

        // ��������� ������ ��������� ��������
        foreach (var pet in selectedPets)
        {
            var petUI = Instantiate(petUIPrefab, selectedPetsContainer);
            var petButton = petUI.GetComponent<PetButton>();
            petButton.SetPet(pet);

            petUI.GetComponent<Button>().onClick.AddListener(() => DeselectPet(pet));
        }

        // ��������� ������ ���� �������� (��� ���������������)
        foreach (var pet in allPets)
        {
            var petUI = Instantiate(petUIPrefab, allPetsContainer);
            var petButton = petUI.GetComponent<PetButton>();
            petButton.SetPet(pet);

            petUI.GetComponent<Button>().onClick.AddListener(() => SelectPet(pet));
        }
    }

    public void AddPet(Pet pet)
    {
        allPets.Add(pet);
        notificationIcon.SetNotification(true);
        UpdateUI();
    }

    public void SelectPet(Pet pet)
    {
        if (selectedPets.Count < maxSelectedPets && !selectedPets.Contains(pet))
        {
            selectedPets.Add(pet);
            allPets.Remove(pet);
            SpawnPetPrefab(pet);
            UpdateUI();
            UpdatePetMultiplier(); // ��������� ��������� ��������
        }
    }

    public void DeselectPet(Pet pet)
    {
        if (selectedPets.Contains(pet))
        {
            selectedPets.Remove(pet);
            allPets.Add(pet);
            RemovePetPrefab(pet);
            UpdateUI();
            UpdatePetMultiplier(); // ��������� ��������� ��������
        }
    }
    private void UpdatePetMultiplier()
    {
        float totalPetMultiplier = 1f; // ��������� ���������

        foreach (var pet in selectedPets)
        {
            totalPetMultiplier += pet.Power; // ��������� ��������� ���� ��������� ��������
        }

        ClickMultiplier.Instance.SetPetMultiplier(totalPetMultiplier);
    }

    private void SpawnPetPrefab(Pet pet)
    {
        if (pet.Prefab != null)
        {
            // ������� ��������� �������
            GameObject spawnedPet = Instantiate(pet.Prefab);
            petOrbitManager.AddPet(spawnedPet); // ��������� � �������� ��������
            activePets[pet] = spawnedPet;
        }
    }

    private void RemovePetPrefab(Pet pet)
    {
        if (activePets.ContainsKey(pet))
        {
            petOrbitManager.RemovePet(activePets[pet]); // ������� �� ��������� ��������
            activePets.Remove(pet);
        }
    }

    public void EquipBestPets()
    {
        // ������� ���� ������� ��������
        foreach (var pet in new List<Pet>(selectedPets))
        {
            DeselectPet(pet);
        }

        // ��������� �������� �� ����
        var bestPets = new List<Pet>(allPets);
        bestPets.Sort((a, b) => b.Power.CompareTo(a.Power)); // ���������� �� �������� ����

        // �������� �������� ��������� ��������
        for (int i = 0; i < Mathf.Min(maxSelectedPets, bestPets.Count); i++)
        {
            SelectPet(bestPets[i]);
        }
    }

    public void OpenPetPanel()
    {
        petPanel.gameObject.SetActive(true); // ��������� ������
        notificationIcon.SetNotification(false); // ������� ������ �����������
        UpdateUI(); // ��������� ���������
    }
}
